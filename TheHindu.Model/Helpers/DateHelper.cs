using System;

namespace TheHindu.Model.Helpers
{
    public static class DateHelper
    {
        #region Methods

        public static string GetDateDifferenceText(DateTime date)
        {
            string dateDifference;
            var diff = DateTime.UtcNow - date;

            if (diff != null && diff.Days >= 0)
            {
                switch (diff.Days)
                {
                    case 0:
                    case 1:
                        {
                            switch (diff.Hours)
                            {
                                case 0:
                                    {
                                        switch (diff.Minutes)
                                        {
                                            case 0:
                                                dateDifference = "Today";
                                                break;

                                            case 1:
                                                dateDifference = "a minute ago";
                                                break;

                                            default:
                                                dateDifference = string.Format("{0} minutes ago", diff.Minutes);
                                                break;
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        dateDifference = "an hour ago";
                                        break;
                                    }
                                default:
                                    {
                                        dateDifference = string.Format("{0} hours ago", diff.Hours);
                                        break;
                                    }
                            }
                        }
                        break;

                    default:
                        dateDifference = string.Format("{0} days ago", diff.Days);
                        break;
                }
            }
            else
            {
                dateDifference = "Today";
            }

            return dateDifference;
        }

        #endregion Methods
    }
}