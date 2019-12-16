#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Crosstales.Skyboxes3D.MediterraneanFreebies.EditorTask
{
    /// <summary>Reminds the customer to create an UAS review.</summary>
    [InitializeOnLoad]
    public static class ReminderCheck
    {
        private const string ASSET_NAME = "Mediterranean Freebies";
        private const string ASSET_URL = "https://assetstore.unity.com/packages/slug/98721?aid=1011lNGT";
        private const string KEY_REMINDER_DATE = "3DS_CFG_REMINDER_DATE";
        private const string KEY_REMINDER_COUNT = "3DS_CFG_REMINDER_COUNT";

        private const int numberOfDays = 17;
        private const int maxDays = numberOfDays * 2;

        #region Constructor

        static ReminderCheck()
        {
            string lastDate = EditorPrefs.GetString(KEY_REMINDER_DATE);
            int count = EditorPrefs.GetInt(KEY_REMINDER_COUNT) + 1;
            string date = System.DateTime.Now.ToString("yyyyMMdd"); // every day
            //string date = System.DateTime.Now.ToString("yyyyMMddHHmm"); // every minute (for tests)

            if (maxDays <= count && !date.Equals(lastDate))
            {
                //if (count % 1 == 0) // for testing only
                if (count % numberOfDays == 0)
                {
                    int option = EditorUtility.DisplayDialogComplex(ASSET_NAME + " - Reminder",
                                "Please don't forget to rate " + ASSET_NAME + " or even better write a little review – it would be very much appreciated!",
                                "Yes, let's do it!",
                                "Not right now",
                                "Don't ask again!");

                    switch (option)
                    {
                        case 0:
                            Application.OpenURL(ASSET_URL);
                            count = 9999;

                            Debug.LogWarning("<color=red>" + Common.Util.BaseHelper.CreateString("❤", 400) + "</color>");
                            Debug.LogWarning("<b>+++ Thank you for rating <color=blue>" + ASSET_NAME + "</color>! +++</b>");
                            Debug.LogWarning("<color=red>" + Common.Util.BaseHelper.CreateString("❤", 400) + "</color>");
                            break;
                        case 1:
                            // do nothing!
                            break;
                        default:
                            count = 9999;
                            break;
                    }
                }

                EditorPrefs.SetString(KEY_REMINDER_DATE, date);
                EditorPrefs.SetInt(KEY_REMINDER_COUNT, count);
            }
        }

        #endregion

    }
}
#endif
// © 2019 crosstales LLC (https://www.crosstales.com)