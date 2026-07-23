import { useCallback, useEffect, useState } from "react";
import { Building2, Plus, Trash2 } from "lucide-react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
import AppButton from "../../components/common/AppButton";
import AppInput from "../../components/common/AppInput";
import AppLoader from "../../components/common/AppLoader";
import AppModal from "../../components/common/AppModal";
import AppTextarea from "../../components/common/AppTextarea";
import ConfirmDialog from "../../components/common/ConfirmDialog";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import type { Company } from "../../models/company";
import { getErrorMessage } from "../../services/apiClient";
import {
  createCompany,
  deleteCompany,
  getCompanies,
} from "../../services/companyService";

const companySchema = z.object({
  name: z.string().trim().min(2, "Enter the company name.").max(200),
  website: z
    .string()
    .trim()
    .refine(
      (value) => !value || /^https?:\/\/.+/i.test(value),
      "Enter a full website URL.",
    ),
  address: z.string().trim().max(300),
  description: z.string().trim().max(1500),
});

type CompanyValues = z.infer<typeof companySchema>;

export default function CompaniesPage() {
  const [companies, setCompanies] = useState<Company[]>([]);
  const [companyToDelete, setCompanyToDelete] = useState<Company | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const {
    register,
    reset,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<CompanyValues>({
    resolver: zodResolver(companySchema),
    defaultValues: { name: "", website: "", address: "", description: "" },
  });

  const loadCompanies = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setCompanies(await getCompanies());
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadCompanies();
  }, [loadCompanies]);

  const onSubmit = handleSubmit(async (values) => {
    setError("");
    try {
      await createCompany({
        name: values.name,
        website: values.website || null,
        address: values.address || null,
        description: values.description || null,
      });
      setModalOpen(false);
      reset();
      await loadCompanies();
    } catch (createError) {
      setError(getErrorMessage(createError));
    }
  });

  const removeCompany = async () => {
    if (!companyToDelete) return;
    setDeleting(true);
    setError("");
    try {
      await deleteCompany(companyToDelete.id);
      setCompanyToDelete(null);
      await loadCompanies();
    } catch (deleteError) {
      setError(getErrorMessage(deleteError));
    } finally {
      setDeleting(false);
    }
  };

  const columns: TableColumn<Company>[] = [
    {
      key: "company",
      header: "Company",
      render: (company) => (
        <div>
          <div className="table-primary">{company.name}</div>
          <div className="table-secondary">
            {company.website || "No website"}
          </div>
        </div>
      ),
    },
    {
      key: "address",
      header: "Address",
      render: (company) => company.address || "Not provided",
    },
    {
      key: "description",
      header: "Description",
      render: (company) => company.description || "Not provided",
    },
    {
      key: "actions",
      header: "Actions",
      align: "right",
      render: (company) => (
        <div className="table-actions">
          <AppButton
            variant="ghost"
            size="small"
            icon={<Trash2 size={15} aria-hidden="true" />}
            onClick={() => setCompanyToDelete(company)}
          >
            Delete
          </AppButton>
        </div>
      ),
    },
  ];

  return (
    <div className="animate-in">
      <PageHeader
        title="Companies"
        description="Manage organizations connected to recruiters and vacancies."
        actions={
          <AppButton
            icon={<Plus size={17} aria-hidden="true" />}
            onClick={() => setModalOpen(true)}
          >
            Add company
          </AppButton>
        }
      />

      {error ? (
        <div className="alert alert--error" role="alert">
          {error}
        </div>
      ) : null}

      <section className="content-panel">
        {loading ? (
          <div className="state-panel">
            <AppLoader label="Loading companies" />
          </div>
        ) : error && companies.length === 0 ? (
          <ErrorState message={error} onRetry={loadCompanies} />
        ) : (
          <DataTable
            columns={columns}
            data={companies}
            getRowKey={(company) => company.id}
            emptyTitle="No companies"
            emptyDescription="Add the first organization to begin creating vacancies."
          />
        )}
      </section>

      <AppModal
        open={modalOpen}
        title="Add company"
        onClose={() => setModalOpen(false)}
        footer={
          <>
            <AppButton variant="secondary" onClick={() => setModalOpen(false)}>
              Cancel
            </AppButton>
            <AppButton
              type="submit"
              form="company-form"
              loading={isSubmitting}
            >
              Add company
            </AppButton>
          </>
        }
      >
        <form
          className="stack"
          id="company-form"
          noValidate
          onSubmit={onSubmit}
        >
          <AppInput
            label="Company name"
            icon={<Building2 size={18} aria-hidden="true" />}
            error={errors.name?.message}
            {...register("name")}
          />
          <AppInput
            label="Website"
            placeholder="https://company.example"
            error={errors.website?.message}
            {...register("website")}
          />
          <AppInput
            label="Address"
            error={errors.address?.message}
            {...register("address")}
          />
          <AppTextarea
            label="Description"
            error={errors.description?.message}
            {...register("description")}
          />
        </form>
      </AppModal>

      <ConfirmDialog
        open={Boolean(companyToDelete)}
        title="Delete company"
        message={`Delete ${companyToDelete?.name || "this company"}? This action cannot be undone.`}
        confirmLabel="Delete company"
        loading={deleting}
        onClose={() => setCompanyToDelete(null)}
        onConfirm={() => void removeCompany()}
      />
    </div>
  );
}
