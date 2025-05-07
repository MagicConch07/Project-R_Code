namespace GM.Data
{
    public class StaffInfo
    {
        public StaffProfile Profile => _profile;
        public int Wages => _wages;

        private StaffProfile _profile;
        private int _wages;

        public StaffInfo(StaffProfile profile, int Wages)
        {
            _profile = profile;
            _wages = Wages;
        }
    }
}
