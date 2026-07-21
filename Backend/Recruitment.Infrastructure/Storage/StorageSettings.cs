namespace Recruitment.Infrastructure.Storage
{
    public class StorageSettings
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string ContainerName { get; set; } = "uploads";

        public string Container
        {
            get => ContainerName;
            set => ContainerName = value;
        }
    }
}
