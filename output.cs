using System;
namespace PetriTypeCheck {
    public class User  : IEmailRecipient {
        public string FirstName {get;} 
        public int Age {get;} 
    }
    public class Company  {
        public string Name {get;} 
        public int PostCode {get;} 
    }
    public class Order  {
        public string OrderNumber {get;} 
    }
    public class Email  {
        public int StatusCode {get;} 
    }
    public interface IEmailRecipient  {
        public string Name {get;} 
        public string Email {get;} 
    }
    public class Net {
        public void Check() {
            User in1 = null;
            Order in2 = null;
            Email out1 = null;
            var send_email_res = send_email ( in1, in2 );
            out1 = send_email_res;
        }
        Email send_email ( IEmailRecipient inp_recipient, Order inp_order ) {
            throw new Exception();
        }
    }
}
