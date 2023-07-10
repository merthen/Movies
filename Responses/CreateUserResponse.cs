namespace Movies.Responses
{
    public class CreateUserResponse
    {
        public int UserId { get; }

        public CreateUserResponse(int id)
        {
            UserId = id;
        }
    }
}
