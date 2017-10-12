using System;

namespace XamCam.Functions.Models
{
	public class CreateAssetFileBody
	{
		public string IsEncrypted { get; set; }
		public string IsPrimary { get; set; }
		public string MimeType { get; set; }
		public string Name { get; set; }
		public string ParentAssetId { get; set; }
	}

	public class AssetDetails
	{
		public Metadata __metadata { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public string ContentFileSize { get; set; }
		public string ParentAssetId { get; set; }
		public object EncryptionVersion { get; set; }
		public object EncryptionScheme { get; set; }
		public bool IsEncrypted { get; set; }
		public object EncryptionKeyId { get; set; }
		public object InitializationVector { get; set; }
		public bool IsPrimary { get; set; }
		public DateTime LastModified { get; set; }
		public DateTime Created { get; set; }
		public string MimeType { get; set; }
		public object ContentChecksum { get; set; }
		public int Options { get; set; }
	}

	public class CreatedAssetRoot
	{
		public AssetDetails d { get; set; }
	}
}