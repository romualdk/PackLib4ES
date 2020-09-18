#region Using directives
using System.Collections.Generic;
using System.Windows.Forms;

using Pic.Plugin;
using Pic.DAL.SQLite;
#endregion

namespace PicParam
{
    public class ProfileLoaderImpl : ProfileLoader
    {
        #region Constructor
        public ProfileLoaderImpl()
        {
        }
        #endregion

        #region Public properties
        public Pic.DAL.SQLite.Component Component
        {
            set
            {
                _comp = value; _dictMajoration = null;
            }
        }
        #endregion

        #region ProfileLoader overrides
       public override void EditMajorations()
        {
            // show majoration edit form
            FormEditMajorations dlg = new FormEditMajorations(_comp.ID, _selectedProfile, this);
            if (DialogResult.OK == dlg.ShowDialog())  {}
        }

        protected override Profile[] LoadProfiles()
        {
            PPDataContext db = new PPDataContext();
            CardboardProfile[] profiles = CardboardProfile.GetAll(db);
            List<Profile> listProfile = new List<Profile>();
            foreach (CardboardProfile dbProfile in profiles)
                listProfile.Add(new Profile(dbProfile.Name));
            if (listProfile.Count > 0)
                Selected = listProfile[0];
            return listProfile.ToArray();
        }
        protected override Dictionary<string, double> LoadMajorationList()
        {
            if (null == Selected || null == _comp)
                return new Dictionary<string, double>();
            if (null == _dictMajoration)
            {
                PPDataContext db = new PPDataContext();

                CardboardProfile selectedProfile = CardboardProfile.GetByName(db, _selectedProfile.Name);
                _dictMajoration = Pic.DAL.SQLite.Component.GetDefaultMajorations(
                    db,
                    _comp.ID,
                    selectedProfile,
                    // rounding to be applied while building majoration dictionary
                    Pic.DAL.SQLite.Component.IntToMajoRounding(Properties.Settings.Default.MajorationRounding)
                    );
            }
            return _dictMajoration;
        }

        public override void SetComponent(Pic.Plugin.Component comp)
        {
            if (null == comp) return;
            _dictMajoration = null;
            PPDataContext db = new PPDataContext();
            _comp = Pic.DAL.SQLite.Component.GetByGuid(db, comp.Guid);
        }

        public override void BuildCardboardProfileDictionary()
        {
            PPDataContext db = new PPDataContext();
            CardboardProfile[] profiles = CardboardProfile.GetAll(db);

            _dictMajoration = null;
        }
        #endregion

        #region Data members
        private Pic.DAL.SQLite.Component _comp;
        #endregion
    }
}
