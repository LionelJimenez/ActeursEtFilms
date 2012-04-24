using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace ActeursETFilms
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public ListCollectionView ViewFilt;
        public ObservableCollection<acteurs> ObsAct;
        public PropertyGroupDescription SexFilter = new PropertyGroupDescription("sexe");
        public DataClasses1DataContext db;
        static List<acteurs_films> ConflictsListAF = new List<acteurs_films>();
        List<object> inserts;
        static int id;
        public MainWindow()
        {
            InitializeComponent();

            db = new DataClasses1DataContext();
            ObsAct = new ObservableCollection<acteurs>();
            ChangeSet cs = db.GetChangeSet();
            inserts = new List<object>(cs.Inserts);
        }
                 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var ActItem = from Act in db.acteurs
                          select Act;

            foreach (acteurs actr in ActItem)
            {
                ObsAct.Add(actr);
            }
            ViewFilt = CollectionViewSource.GetDefaultView(ObsAct) as ListCollectionView;

            LBActeurs.ItemsSource = ObsAct;
            LBActeurs.DisplayMemberPath = "nom";
            
        }

        private void LBActeurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            ListBox actrs = (ListBox)sender;
            acteurs act = (acteurs)actrs.SelectedItem;
            if (act == null)
            {
                id = 0;
            }
            else
            id = act.id_acteur;

            var MFlm = from Act in db.acteurs
                       from AF in db.acteurs_films
                       from Film in db.films
                       where Act.id_acteur == id
                       where Act.id_acteur == AF.id_acteur
                       where AF.id_film == Film.id_film
                       select Film;

            
            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.DisplayMemberBinding = new Binding("nom");
            gvc1.Header = "Nom du film";
            DGFilmograph.ItemsSource = MFlm;
            
        }

        private void TBNom_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void DGFilmograph_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButFiltAge_Click(object sender, RoutedEventArgs e)
        {
            if (TbAgeFil.Text != null)
            {
                try
                {
                    int age = Int32.Parse(TbAgeFil.Text);

                    FiltreAge Fa = new FiltreAge(age);
                    ViewFilt = CollectionViewSource.GetDefaultView(ObsAct) as ListCollectionView;
                    if (ViewFilt != null)
                        ViewFilt.Filter = new Predicate<object>(Fa.filtr);

                }
                catch(Exception)
                {
                    MessageBox.Show("Veuillez entrer un chiffre");
                }
            }
        }

        private void ButNOTFiltAge_Click(object sender, RoutedEventArgs e)
        {

            ViewFilt.Filter = null;
        }

        private void BtnGrouper_Click(object sender, RoutedEventArgs e)
        {
            
            ViewFilt = CollectionViewSource.GetDefaultView(ObsAct) as ListCollectionView;
            ViewFilt.GroupDescriptions.Add(SexFilter);

        }

        private void BtnNPGrouper_Click(object sender, RoutedEventArgs e)
        {
            ViewFilt = CollectionViewSource.GetDefaultView(ObsAct) as ListCollectionView;
            ViewFilt.GroupDescriptions.Remove(SexFilter);
        }
        public static void RollbackChanges(DataContext dataContext)
        {
            ChangeSet changeSet = dataContext.GetChangeSet();
            List<object> inserts = new List<object>(changeSet.Inserts);
            List<object> deleteAndUpdate = new List<object>(changeSet.Deletes);
            deleteAndUpdate.AddRange(changeSet.Updates);

            foreach (object inserted in inserts)
            {
                ITable table = dataContext.GetTable(inserted.GetType());
                table.DeleteOnSubmit(inserted);
            }

            dataContext.Refresh(RefreshMode.OverwriteCurrentValues, deleteAndUpdate.ToArray());
        }

        private void test_Click(object sender, RoutedEventArgs e)
        {

            ChangeSet cs = db.GetChangeSet();

            Dictionary<films, acteurs_films> IFaf = new Dictionary<films, acteurs_films>();
            List<acteurs_films> IAf = new List<acteurs_films>();
            List<acteurs_films> DLaf = new List<acteurs_films>();

            int idf; 
            int? x = 0;
            int? y = 0;
            int? res= 0;
            db.LastId(ref x,ref y,ref res);
            int flagF = Int32.Parse(res.ToString());


            foreach (var deletes in cs.Deletes)
            {

                if (deletes is films)
                {
                    films del = (films)deletes;

                    acteurs_films actf = new acteurs_films();
                    actf.id_acteur = id;
                    actf.id_film = del.id_film;
                    DLaf.Add(actf);
                }
            }

            foreach (var updates in cs.Updates)
            {
                if (updates is films)
                {
                    db.SubmitChanges();
                    MessageBox.Show("Opération d'update effectuée");
                }
            }

            foreach (var insert in cs.Inserts)
            {
                if (insert is films)
                {
                    films ins = (films)insert;
                    // Le film existe-t'il déjà?
                    idf = 0;
                    
                    try
                    {
                        films idflms = (from Flms in db.films
                                        where ins.nom == Flms.nom
                                        select Flms).First();
                        idf = idflms.id_film;
                    }

                    catch (InvalidOperationException)
                    {
                        idf = 0;
                    }

                    // Si non présent => On le rajoute à la table ainsi qu'à l'acteur
                    if (idf == 0)
                    {
                        try
                        {
                            bool erreur = false;
                            films fm = new films { annee = ins.annee, nom = ins.nom, realisateur = ins.realisateur };

                            foreach (KeyValuePair<films, acteurs_films> kvp in IFaf)
                            {
                                if (kvp.Key.nom == fm.nom)
                                    erreur = true;
                            }
                            if (erreur==false)
                            {
                                acteurs_films actf = new acteurs_films();
                                actf.id_acteur = id;
                                actf.id_film = flagF;

                                IFaf.Add(fm, actf);
                                flagF++;
                            }
                            erreur = false;
                        }
                        catch (Exception)
                        { }
                    }

                    // Si oui => Ajout du film à l'acteur  
                    else
                    {
                        bool erreur = false;
                        
                        // Je vérifie que je ne l'encode pas 2 fois
                        var MFlm = from Act in db.acteurs
                                   from AF in db.acteurs_films
                                   from Film in db.films
                                   where Act.id_acteur == id
                                   where Act.id_acteur == AF.id_acteur
                                   where AF.id_film == Film.id_film
                                   select Film;

                        foreach (films fitem in MFlm)
                        {
                            if (ins.id_film == fitem.id_film)
                            {
                                erreur = true;
                            }
                        }

                        //Si unique=> j'insère
                        if (erreur == false)
                        {
                                acteurs_films actf = new acteurs_films();
                                actf.id_acteur = id;
                                actf.id_film = idf;
                                IAf.Add(actf);
                        }      
                    }   
                }
            }
            
            //On nettoie le datacontext => insertion àpd Objects
            db = new DataClasses1DataContext();

            //Suppression
            if (DLaf.Count != 0)
            {
                acteurs_films af = new acteurs_films();
                foreach (acteurs_films AfItem in DLaf)
                {
                    try
                    {
                        af.films = null;
                        af.acteurs = null;
                        af.id_acteur = AfItem.id_acteur;
                        af.id_film = AfItem.id_film;

                        db.acteurs_films.Attach(af);
                        db.acteurs_films.DeleteOnSubmit(af);
                    }
                    catch (Exception)
                    {
                        db = new DataClasses1DataContext();
                        db.acteurs_films.Attach(AfItem);
                        db.acteurs_films.DeleteOnSubmit(AfItem);
                        db.SubmitChanges();
                        
                    }
                }

                MessageBox.Show("Opération de suppression effectuée");
                db.SubmitChanges();
            }

            
            
            // Insertion nouveau film
            if (IFaf.Count != 0)
            {

                foreach (KeyValuePair<films, acteurs_films> kvp in IFaf)
                {
                    db.films.InsertOnSubmit(kvp.Key);
                    db.acteurs_films.InsertOnSubmit(kvp.Value);
                }

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception)
                {
                    MessageBox.Show("Doublons interdits-veuillez recommencer");
                }
            }

                // Insertion Film existant
              if (IAf.Count != 0)
              {
                  foreach (acteurs_films AfItem in IAf)
                  {
                      db.acteurs_films.InsertOnSubmit(AfItem);
                  }
                  try
                  {
                      db.SubmitChanges();
                  }
                  catch (Exception)
                  {
                      MessageBox.Show("Doublons interdits-veuillez recommencer");
                  }
              }
              if (IAf.Count != 0 || IFaf.Count != 0)
              {
                  MessageBox.Show("Opération d'insertion effectuée");
              }
            cs = db.GetChangeSet();
            db = new DataClasses1DataContext();
        }

    }
}
