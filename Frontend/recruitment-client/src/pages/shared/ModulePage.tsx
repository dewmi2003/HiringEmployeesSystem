import type { LucideIcon } from "lucide-react";
import { Settings } from "lucide-react";
import EmptyState from "../../components/common/EmptyState";
import PageHeader from "../../components/layout/PageHeader";

interface ModulePageProps {
  title: string;
  description: string;
  message: string;
  icon?: LucideIcon;
}

export default function ModulePage({
  title,
  description,
  message,
  icon = Settings,
}: ModulePageProps) {
  return (
    <div className="animate-in">
      <PageHeader title={title} description={description} />
      <section className="content-panel">
        <EmptyState title={title} description={message} icon={icon} />
      </section>
    </div>
  );
}
