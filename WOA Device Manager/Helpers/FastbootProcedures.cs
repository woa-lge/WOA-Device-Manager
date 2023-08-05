﻿using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using WOADeviceManager.Managers;

namespace WOADeviceManager.Helpers
{
    internal class FastbootProcedures
    {
        public static string GetProduct(string deviceName)
        {
            string productGetVar = FastbootManager.SendFastbootCommand("getvar product");
            if (productGetVar.Contains(Environment.NewLine))
            {
                string firstLine = productGetVar.Split(Environment.NewLine)[0];
                if (firstLine.Contains(":"))
                {
                    return firstLine.Split(": ")[1];
                }
            }
            return null;
        }

        public static void Reboot(string deviceName)
        {
            FastbootManager.SendFastbootCommand("reboot");
        }

        public static async Task<bool> FlashUnlock(string deviceName, Control frameHost = null)
        {
            if (FastbootManager.SendFastbootCommand("flashing get_unlock_ability").Contains("get_unlock_ability: 1"))
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "⚠️ EVERYTHING WILL BE FORMATTED";
                dialog.Content = "Flash unlocking requires everything to be formatted. MAKE SURE YOU HAVE MADE A COPY OF EVERYTHING. We're not responsible for data loss.";
                dialog.PrimaryButtonText = "⚠️ Proceed";
                dialog.PrimaryButtonClick += (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
                {
                    Debug.WriteLine("hi");
                    // TODO: Disabled for safety
                    //return FastbootManager.SendFastbootCommand("flashing unlock") != null; // TODO: error handling here, always returns true rn ofc
                    dialog.Hide();
                };
                dialog.CloseButtonText = "Cancel";
                if (frameHost != null)
                {
                    dialog.XamlRoot = frameHost.XamlRoot;
                }
                _ = dialog.ShowAsync();
                return true;
            } 
            else
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "Unlocking is disabled";
                dialog.Content = "Flash Unlocking is disabled from Developer Settings in Android. Please enable it manually from there.";
                dialog.CloseButtonText = "OK";
                if (frameHost != null)
                {
                    dialog.XamlRoot = frameHost.XamlRoot;
                }
                _ = dialog.ShowAsync();
                return false;
            }
            
        }

        public static bool FlashLock(string deviceName, Control frameHost = null)
        {
            // TODO: Check that the device doesn't have Windows installed
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "⚠️ Your bootloader will be locked";
            dialog.Content = "This procedure will lock your bootloader. You usually don't want to do this unless you have to sell your device.";
            dialog.PrimaryButtonText = "⚠️ Proceed";
            dialog.PrimaryButtonClick += (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                Debug.WriteLine("hi");
                // TODO: Disabled for safety
                //FastbootManager.SendFastbootCommand("flashing lock") != null; // TODO: error handling here, always returns true rn ofc
                dialog.Hide();
            };
            dialog.CloseButtonText = "Cancel";
            if (frameHost != null)
            {
                dialog.XamlRoot = frameHost.XamlRoot;
            }
            _ = dialog.ShowAsync();
            return true; 
        }

        public static async Task<bool> BootTWRP(string deviceName)
        {
            StorageFile twrp = await ResourcesManager.RetrieveFile(ResourcesManager.DownloadableComponent.TWRP);
            if (twrp == null) return false;

            return FastbootManager.SendFastbootCommand($"boot {twrp.Path}") != null;
        }
    }
}
