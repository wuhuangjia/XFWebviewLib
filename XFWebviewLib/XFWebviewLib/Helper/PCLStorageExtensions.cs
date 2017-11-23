using System;
namespace XFWebviewLib.Helper
{
    using System.Threading;
    using PCLStorage;
    using FileAccess = PCLStorage.FileAccess;

    public static class PCLStorageExtensions
    {
        public static async void CopyFileTo(this IFile file, IFolder destinationFolder, CancellationToken cancellationToken = default(CancellationToken))
        {
            var destinationFile =
                await destinationFolder.CreateFileAsync(file.Name, CreationCollisionOption.ReplaceExisting, cancellationToken);

            using (var outFileStream = await destinationFile.OpenAsync(FileAccess.ReadAndWrite, cancellationToken))
            using (var sourceStream = await file.OpenAsync(FileAccess.Read, cancellationToken))
            {
                await sourceStream.CopyToAsync(outFileStream, 81920, cancellationToken);
            }
        }
    }
}
