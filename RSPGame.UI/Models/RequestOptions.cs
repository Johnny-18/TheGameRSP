namespace RSPGame.UI.Models
{
    public class RequestOptions
    {
        public string Address { get; set; }

        public RequestMethod Method { get; set; }

        public string Token { get; set; }
        
        public string Body { get; set; }
        
        public bool IsValid {
            get
            {
                if (string.IsNullOrEmpty(Address) || Method == RequestMethod.Undefined)
                    return false;
                
                return true;
            }
        }
    }
}