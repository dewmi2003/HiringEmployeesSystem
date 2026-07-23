import AppButton from "./AppButton";
import AppModal from "./AppModal";

interface ConfirmDialogProps {
  open: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  loading?: boolean;
  onConfirm: () => void;
  onClose: () => void;
}

export default function ConfirmDialog({
  open,
  title,
  message,
  confirmLabel = "Confirm",
  loading = false,
  onConfirm,
  onClose,
}: ConfirmDialogProps) {
  return (
    <AppModal
      open={open}
      title={title}
      onClose={onClose}
      footer={
        <>
          <AppButton variant="secondary" onClick={onClose}>
            Cancel
          </AppButton>
          <AppButton
            variant="danger"
            loading={loading}
            onClick={onConfirm}
          >
            {confirmLabel}
          </AppButton>
        </>
      }
    >
      <p>{message}</p>
    </AppModal>
  );
}
