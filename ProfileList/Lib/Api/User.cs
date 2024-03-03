using ProfileList.Lib.Profile;
using System.Text.Json.Nodes;

namespace ProfileList.Lib.Api
{
    public class User
    {
        /// <summary>
        /// ユーザープロファイルを取得
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static UserProfileCollection Profile(UserParameter parameter = null)
        {
            if (parameter?.Refresh == true)
            {
                Item.Logger.WriteLine("Refrsh, UserProfileCollectionl.");
                Item.UserProfileCollection = new();
            }
            return Item.UserProfileCollection;
        }

        public static UserLogonSessionCollection Session(UserParameter parameter = null)
        {
            if (parameter?.Refresh == true)
            {
                Item.Logger.WriteLine("Refrsh, UserLogonSessionCollection.");
                Item.UserLogonSessionCollection = new();
            }
            return Item.UserLogonSessionCollection;
        }
    }
}
