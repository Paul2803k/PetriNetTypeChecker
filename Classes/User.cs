using NeTypeChecker.Interfaces;

namespace NeTypeChecker.Types
{
    class User : IEmailable
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}
