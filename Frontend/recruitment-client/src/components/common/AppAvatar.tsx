import { getInitials } from "../../utils/formatters";

interface AppAvatarProps {
  name: string;
  large?: boolean;
}

export default function AppAvatar({ name, large = false }: AppAvatarProps) {
  return (
    <span
      className={`avatar${large ? " avatar--large" : ""}`}
      aria-label={name}
      title={name}
    >
      {getInitials(name)}
    </span>
  );
}
