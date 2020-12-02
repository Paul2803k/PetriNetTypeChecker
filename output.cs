using System;
namespace PetriTypeCheck {
    public class User  : IEmailRecipient {
        public string Name {get;} 
        public int Age {get;} 
        public string Email {get;} 
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
            User out2 = null;
            User out3 = null;
            var send_email_res = send_email ( in1, in2 );
            out1 = send_email_res.Item1;
            out2 = send_email_res.Item2;
            var do_nothing_res = do_nothing ( out2 );
            out3 = do_nothing_res;
        }
        Tuple<Email, send_email_genTypeIn_0> send_email <send_email_genTypeIn_0> (send_email_genTypeIn_0 inp_recipient, Order inp_order) where send_email_genTypeIn_0 : IEmailRecipient {
            throw new Exception();
        }
        do_nothing_genTypeIn_0 do_nothing <do_nothing_genTypeIn_0> (do_nothing_genTypeIn_0 inp_nothing) where do_nothing_genTypeIn_0 : class {
            throw new Exception();
        }
    }
}
