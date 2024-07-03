using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Web;
using WOADeviceManager.Helpers;

namespace WOADeviceManager.Managers
{
    public class ResourcesManager
    {
        public enum DownloadableComponent
        {
            DRIVERS_MH2LM,
            FD_MH2LM,
            FD_SECUREBOOT_DISABLED_MH2LM,
            PARTED,
            TWRP_MH2LM,
            UEFI_MH2LM,
            UEFI_SECUREBOOT_DISABLED_MH2LM
        }

        // None of these things are how it should still be
        public static async Task<StorageFile> RetrieveFile(DownloadableComponent component, bool redownload = false)
        {
            string downloadPath = string.Empty;
            string fileName = string.Empty;
            string releaseVersion = string.Empty;

            switch (component)
            {
                case DownloadableComponent.PARTED:
                    downloadPath = "https://github.com/woa-lge/Port-Windows-11-Lge-devices/raw/main/Files/parted";
                    fileName = "parted";
                    break;

                case DownloadableComponent.TWRP_MH2LM:
                    downloadPath = "https://github.com/woa-lge/Port-Windows-11-Lge-devices/raw/main/Files/g8x-twrp.img";
                    fileName = "g8x-twrp.img";
                    break;
                case DownloadableComponent.UEFI_MH2LM:
                    releaseVersion = await HttpsUtils.GetLatestBSPReleaseVersion();
                    downloadPath = $"https://github.com/woa-lge/G8x-Releases/releases/download/{releaseVersion}/lg-mh2lm.UEFI-v{releaseVersion}.Fast.Boot.zip";
                    fileName = $"lg-mh2lm.UEFI-v{releaseVersion}.Fast.Boot.zip";
                    break;
                case DownloadableComponent.UEFI_SECUREBOOT_DISABLED_MH2LM:
                    releaseVersion = await HttpsUtils.GetLatestBSPReleaseVersion();
                    downloadPath = $"https://github.com/woa-lge/G8x-Releases/releases/download/{releaseVersion}/lg-mh2lm.UEFI-v{releaseVersion}.Secure.Boot.Disabled.Fast.Boot.zip";
                    fileName = $"lg-mh2lm.UEFI-v{releaseVersion}.Secure.Boot.Disabled.Fast.Boot.zip";
                    break;
                case DownloadableComponent.FD_MH2LM:
                    releaseVersion = await HttpsUtils.GetLatestBSPReleaseVersion();
                    downloadPath = $"https://github.com/woa-lge/G8x-Releases/releases/download/{releaseVersion}/lg-mh2lm.UEFI-v{releaseVersion}.FD.for.making.your.own.Dual.Boot.Image.zip";
                    fileName = $"lg-mh2lm.UEFI-v{releaseVersion}.FD.for.making.your.own.Dual.Boot.Image.zip";
                    break;
                case DownloadableComponent.FD_SECUREBOOT_DISABLED_MH2LM:
                    releaseVersion = await HttpsUtils.GetLatestBSPReleaseVersion();
                    downloadPath = $"https://github.com/woa-lge/G8x-Releases/releases/download/{releaseVersion}/lg-mh2lm.UEFI-v{releaseVersion}.Secure.Boot.Disabled.FD.for.making.your.own.Dual.Boot.Image.zip";
                    fileName = $"lg-mh2lm.UEFI-v{releaseVersion}.Secure.Boot.Disabled.FD.for.making.your.own.Dual.Boot.Image.zip";
                    break;
                case DownloadableComponent.DRIVERS_MH2LM:
                    releaseVersion = await HttpsUtils.GetLatestBSPReleaseVersion();
                    downloadPath = $"https://github.com/woa-lge/G8x-Releases/releases/download/{releaseVersion}/Lg-Drivers-v{releaseVersion}-Desktop-Mh2lm.7z";
                    fileName = $"Lg-Drivers-v{releaseVersion}-Desktop-Mh2lm.7z";
                    break;
            }
            return await RetrieveFile(downloadPath, fileName, redownload);
        }

        public static async Task<StorageFile> RetrieveFile(string path, string fileName, bool redownload = false)
        {
            if (redownload || !IsFileAlreadyDownloaded(fileName))
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using HttpClient client = new();
                using Task<Stream> webStream = client.GetStreamAsync(new Uri(path));
                using FileStream fs = new(file.Path, FileMode.OpenOrCreate);
                webStream.Result.CopyTo(fs);
                return file;
            }
            else
            {
                return await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            }
        }

        public static bool IsFileAlreadyDownloaded(string fileName)
        {
            return File.Exists(ApplicationData.Current.LocalFolder.Path + "\\" + fileName);
        }
    }
}
