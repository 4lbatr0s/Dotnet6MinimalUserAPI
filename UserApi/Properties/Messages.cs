namespace UserApi.Properties
{
    //my messages.
    public class Messages
    {
        public static string UserIsNotFound = "User is not found in database.";
        public static string  UserIsDeleted(int id){
            return ($"User with the id of {id} is deleted from database");
        }

        public static string UserIsUpdated(int id)
        {
            return ($"User with the id of {id} is updated.");
        }
    }
}
