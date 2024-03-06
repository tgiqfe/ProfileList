using ProfileList.Lib.Profile;

namespace ProfileList.Lib.Api
{
    public class Profile
    {
        /// <summary>
        /// ユーザープロファイルを取得
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static UserProfileCollection List(ProfileParameter parameter = null)
        {
            if (parameter?.Refresh == true)
            {
                Item.Logger.WriteLine("Refrsh, UserProfileCollectionl.");
                Item.UserProfileCollection = new();
            }
            Item.Logger.WriteLine($"Summary: profile count [{Item.UserProfileCollection.GetSummary_UserCount()}]");
            Item.Logger.WriteLine($"Summary: profile users [{Item.UserProfileCollection.GetSummary_Users()}]");

            return Item.UserProfileCollection;
        }

        /// <summary>
        /// ユーザープロファイルを削除
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static dynamic Delete(ProfileParameter parameter = null)
        {
            Item.Logger.WriteLine("Refrsh, UserProfileCollectionl.");
            Item.UserProfileCollection = new();

            List<UserProfile> targetList = null;
            if (parameter?.All == true)
            {
                Item.Logger.WriteLine("Delete all user profiles. (exclude protected user profile.)");
                Item.Logger.WriteLine($"Protected user profile: {Item.Setting.ProtectedProfile}");
                targetList = Item.UserProfileCollection.Profiles.
                    Where(x => !Item.Setting.ProtectedProfileUsers.Contains(x.UserName)).
                    Where(x => !x.IsLogon).
                    ToList();
                targetList.ForEach(x => x.Delete());
            }
            else
            {
                string username = parameter?.UserName;
                if (!string.IsNullOrEmpty(username))
                {
                    targetList = Item.UserProfileCollection.Profiles.
                        Where(x => x.UserName == username).
                        Where(x => !x.IsLogon).
                        ToList();
                    Item.Logger.WriteLine($"Delete user profile. [{username}]");
                    if (targetList.Any(x => Item.Setting.ProtectedProfileUsers.Contains(x.UserName)))
                    {
                        Item.Logger.WriteLine($"Profile delete is skipped. (protected user profile)");
                        Item.Logger.WriteLine($"Protected user profile: {Item.Setting.ProtectedProfile}");
                        return new
                        {
                            DeleteProfile = new string[] { },
                        };
                    }
                    targetList.ForEach(x => x.Delete());
                }
                else
                {
                    Item.Logger.WriteLine("Profile delete is failed. (need [all] or [username])");
                    return new
                    {
                        DeleteProfile = new string[] { },
                    };
                }
            }

            return new
            {
                DeleteProfile = targetList.Select(x => $"{x.UserDomain}\\{x.UserName}")
            };
        }
    }
}
