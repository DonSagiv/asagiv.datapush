using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Text;
using Java.IO;
using System;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.mobile.Droid
{
    public static class FilesHelper
    {
        #region Fields
        private const string _externalStorageAuthority = "com.android.externalstorage.documents";
        private const string _downloadsAuthority = "com.android.providers.downloads.documents";
        private const string _mediaAuthority = "com.android.providers.media.documents";
        private const string _photoAuthority = "com.google.android.apps.photos.content";
        private const string _diskAuthority = "com.google.android.apps.docs.storage";
        private const string _diskLegacyAuthority = "com.google.android.apps.docs.storage.legacy";
        #endregion

        /// <summary>
        /// Main feature. Return actual path for file from uri. 
        /// </summary>
        /// <param name="uri">File's uri</param>
        /// <param name="context">Current context</param>
        /// <returns>Actual path</returns>
        public static string GetActualPathForFile(Android.Net.Uri uri, Context context)
        {
            var isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

            if (isKitKat && DocumentsContract.IsDocumentUri(context, uri))
            {
                // ExternalStorageProvider
                if (IsExternalStorageDocument(uri))
                {
                    var docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    var split = docId.Split(chars);
                    var type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }
                }
                // Google Drive
                else if (IsDiskContentUri(uri))
                {
                    return GetDriveFileAbsolutePath(context, uri);
                }
                // DownloadsProvider
                else if (IsDownloadsDocument(uri))
                {
                    try
                    {
                        var id = DocumentsContract.GetDocumentId(uri);

                        if (!TextUtils.IsEmpty(id))
                        {
                            if (id.StartsWith("raw:"))
                            {
                                return id.Replace("raw:", "");
                            }

                            var contentUriPrefixesToTry = new string[]{
                                    "content://downloads/public_downloads",
                                    "content://downloads/my_downloads",
                                    "content://downloads/all_downloads"
                            };

                            string path = null;

                            foreach (var contentUriPrefix in contentUriPrefixesToTry)
                            {
                                var contentUri = ContentUris.WithAppendedId(Android.Net.Uri.Parse(contentUriPrefix), long.Parse(id));

                                path = GetDataColumn(context, contentUri, null, null);

                                if (!string.IsNullOrEmpty(path))
                                {
                                    return path;
                                }
                            }

                            // path could not be retrieved using ContentResolver, therefore copy file to accessible cache using streams
                            var fileName = GetFileName(context, uri);
                            var cacheDir = GetDocumentCacheDir(context);
                            var file = GenerateFileName(fileName, cacheDir);

                            if (file != null)
                            {
                                path = file.AbsolutePath;
                                _ = SaveFileFromUri(context, uri, path);
                            }

                            // last try
                            return string.IsNullOrEmpty(path)
                                ? Android.OS.Environment.ExternalStorageDirectory.ToString() + "/Download/" + GetFileName(context, uri)
                                : path;
                        }
                    }
                    catch
                    {
                        return Android.OS.Environment.ExternalStorageDirectory.ToString() + "/Download/" + GetFileName(context, uri);
                    }
                }
                // MediaProvider
                else if (IsMediaDocument(uri))
                {
                    var docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    var split = docId.Split(chars);

                    var type = split[0];

                    Android.Net.Uri contentUri = null;

                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    var selection = "_id=?";
                    var selectionArgs = new string[] { split[1] };

                    return GetDataColumn(context, contentUri, selection, selectionArgs);
                }

            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                // Return the remote address
                if (IsGooglePhotosUri(uri))
                {
                    return uri.LastPathSegment;
                }

                // Google Disk document .legacy
                return IsDiskLegacyContentUri(uri)
                    ? GetDriveFileAbsolutePath(context, uri)
                    : GetDataColumn(context, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        /// <summary>
        /// Create file in current directory with unique name
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="directory">Current directory</param>
        /// <returns>Created file</returns>
        public static Java.IO.File GenerateFileName(string name, Java.IO.File directory)
        {
            if (name == null)
            {
                return null;
            }

            var file = new Java.IO.File(directory, name);

            if (file.Exists())
            {
                var dotIndex = name.LastIndexOf('.');

                if (dotIndex > 0)
                {
                    var fileName = name.Substring(0, dotIndex);
                    var extension = name.Substring(dotIndex);

                    var index = 0;

                    while (file.Exists())
                    {
                        index++;
                        name = $"{fileName}({index}){extension}";
                        file = new Java.IO.File(directory, name);
                    }
                }
            }

            try
            {
                if (!file.CreateNewFile())
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Figure out what to do after.
            }

            return file;
        }

        /// <summary>
        /// Return file path for specified uri using CacheDir
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="uri">Specified uri</param>
        /// <returns>Drive File absolute path</returns>
        private static string GetDriveFileAbsolutePath(Context context, global::Android.Net.Uri uri)
        {
            ICursor cursor = null;
            FileInputStream input = null;
            FileOutputStream output = null;

            try
            {
                cursor = context.ContentResolver.Query(uri, new string[] { OpenableColumns.DisplayName }, null, null, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    var column_index = cursor.GetColumnIndexOrThrow(OpenableColumns.DisplayName);
                    var fileName = cursor.GetString(column_index);

                    if (uri == null)
                    {
                        return null;
                    }
                        
                    var resolver = context.ContentResolver;

                    var outputFilePath = new Java.IO.File(context.CacheDir, fileName).AbsolutePath;

                    var pfd = resolver.OpenFileDescriptor(uri, "r");

                    var fd = pfd.FileDescriptor;

                    input = new FileInputStream(fd);
                    output = new FileOutputStream(outputFilePath);

                    var read = 0;

                    var bytes = new byte[4096];

                    while ((read = input.Read(bytes)) != -1)
                    {
                        output.Write(bytes, 0, read);
                    }

                    return new Java.IO.File(outputFilePath).AbsolutePath;
                }
            }
            catch (Java.IO.IOException ignored)
            {
                // nothing we can do
            }
            finally
            {
                if (cursor != null)
                {
                    cursor.Close();
                }

                input.Close();
                output.Close();
            }

            return string.Empty;
        }

        /// <summary>
        /// Return filename for specified uri
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="uri">Specified uri</param>
        /// <returns>Filename</returns>
        private static string GetFileName(Context context, global::Android.Net.Uri uri)
        {
            var result = string.Empty;

            if (uri.Scheme.Equals("content"))
            {
                var cursor = context.ContentResolver.Query(uri, null, null, null, null);
                try
                {
                    if (cursor != null && cursor.MoveToFirst())
                    {
                        result = cursor.GetString(cursor.GetColumnIndex(OpenableColumns.DisplayName));
                    }
                }
                finally
                {
                    cursor.Close();
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                result = uri.Path;
                var cut = result.LastIndexOf('/');

                if (cut != -1)
                {
                    result = result.Substring(cut + 1);
                }
            }

            return result;
        }

        /// <summary>
        /// Return app cache directory
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Cache directory</returns>
        private static Java.IO.File GetDocumentCacheDir(Context context)
        {
            var dir = new Java.IO.File(context.CacheDir, "documents");

            if (!dir.Exists())
            {
                dir.Mkdirs();
            }

            return dir;
        }

        /// <summary>
        /// Save file from URI to destination path
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="uri">File URI</param>
        /// <param name="destinationPath">Destination path</param>
        /// <returns>Task for await</returns>
        private async static Task SaveFileFromUri(Context context, global::Android.Net.Uri uri, string destinationPath)
        {
            var stream = context.ContentResolver.OpenInputStream(uri);

            BufferedOutputStream bos = null;

            try
            {
                bos = new BufferedOutputStream(System.IO.File.OpenWrite(destinationPath));

                var bufferSize = 1024 * 4;
                var buffer = new byte[bufferSize];

                while (true)
                {
                    var len = await stream.ReadAsync(buffer, 0, bufferSize);

                    if (len == 0)
                    {
                        break;
                    }

                    await bos.WriteAsync(buffer, 0, len);
                }
            }
            catch (Exception ex)
            {
                // Figure out what to do after.
            }
            finally
            {
                try
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }

                    if (bos != null)
                    {
                        bos.Close();
                    }
                }
                catch (Exception ex)
                {
                    // Figure out what to do after.
                }
            }
        }

        /// <summary>
        /// Return data for specified uri
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="uri">Current uri</param>
        /// <param name="selection">Args names</param>
        /// <param name="selectionArgs">Args values</param>
        /// <returns>Data</returns>
        private static string GetDataColumn(Context context, global::Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            ICursor cursor = null;
            var column = "_data";
            string[] projection = { column };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    var index = cursor.GetColumnIndexOrThrow(column);

                    return cursor.GetString(index);
                }
            }
            catch { }
            finally
            {
                if (cursor != null)
                {
                    cursor.Close();
                }
            }
            return null;
        }

        //Whether the Uri authority is ExternalStorageProvider.
        private static bool IsExternalStorageDocument(global::Android.Net.Uri uri) => _externalStorageAuthority.Equals(uri.Authority);

        //Whether the Uri authority is DownloadsProvider.
        private static bool IsDownloadsDocument(global::Android.Net.Uri uri) => _downloadsAuthority.Equals(uri.Authority);

        //Whether the Uri authority is MediaProvider.
        private static bool IsMediaDocument(global::Android.Net.Uri uri) => _mediaAuthority.Equals(uri.Authority);

        //Whether the Uri authority is Google Photos.
        private static bool IsGooglePhotosUri(global::Android.Net.Uri uri) => _photoAuthority.Equals(uri.Authority);

        //Whether the Uri authority is Google Disk.
        private static bool IsDiskContentUri(global::Android.Net.Uri uri) => _diskAuthority.Equals(uri.Authority);

        //Whether the Uri authority is Google Disk Legacy.
        private static bool IsDiskLegacyContentUri(global::Android.Net.Uri uri) => _diskLegacyAuthority.Equals(uri.Authority);
    }
}