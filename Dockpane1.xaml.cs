using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core;
using ArcGIS.Desktop.Core;


namespace ProAppModule2
{
    /// <summary>
    /// Interaction logic for Dockpane1View.xaml
    /// </summary>
    public partial class Dockpane1View : UserControl
    {
        public Dockpane1View()
        {
            InitializeComponent();
        }

        private async void cbo_change_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mapView = MapView.Active;
            if (mapView == null)
                return;

            lst_fields.Items.Clear();

            var featureLayers = mapView.Map.Layers.OfType<FeatureLayer>();
            IReadOnlyList<ArcGIS.Core.Data.Field> fields = null;
            List<string> fieldList = new List<string>();
            
            var lyr = featureLayers.ElementAt(cbo_change.SelectedIndex);
            var junk = "";

            await ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
            {
                ArcGIS.Core.Data.Table table = lyr.GetTable();
                if (table != null)
                {
                    ArcGIS.Core.Data.TableDefinition def = table.GetDefinition();
                    fields = def.GetFields();

                    foreach (var fld in fields)
                    {
                        if (fld.Name != "Shape")
                        {
                            fieldList.Add(fld.Name);
                        }
                        
                    }
                }
                ArcGIS.Core.Data.FeatureClass featureClass = lyr.GetFeatureClass();
                junk = featureClass.GetCount().ToString();
            });

            txt_cnt.Text = "(" + junk + " recs)";

            for (int i = 0; i < fieldList.Count; i++)
            {
                lst_fields.Items.Add(fieldList[i]);
            }

            txt_value.Text = "";
            btn_go.IsEnabled = false;
            txt_blk.Text = "";
        }

        private void lst_fields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if (lst_fields.SelectedItem != null)
            {
                txt_blk.Text = lst_fields.SelectedItem.ToString() + "  =";
            }
            
            txt_value.Text = "";
            btn_go.IsEnabled = false;
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            cbo_change.Items.Clear();
            lst_fields.Items.Clear();
            
            var mapView = MapView.Active;
            if (mapView == null)
                return;

            var featureLayers = mapView.Map.Layers.OfType<FeatureLayer>();
            IReadOnlyList<ArcGIS.Core.Data.Field> fields = null;
            List<string> fieldList = new List<string>();

            foreach (var lyr in featureLayers.ToList())
            {
                cbo_change.Items.Add(lyr.Name);
            }

            foreach (var lyr in featureLayers.ToList())
            {
                await ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
                {
                    ArcGIS.Core.Data.Table table = lyr.GetTable();
                    if (table != null)
                    {
                        ArcGIS.Core.Data.TableDefinition def = table.GetDefinition();
                        fields = def.GetFields();

                        foreach (var fld in fields)
                        {
                            fieldList.Add(fld.Name);
                        }
                    }
                });

                for (int i = 0; i < fieldList.Count; i++)
                {
                    lst_fields.Items.Add(fieldList[i]);
                }
                break;
            }
            cbo_change.SelectedIndex = 0;
            btn_go.IsEnabled = false;
        }

        private void lst_fields_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private async void btn_go_Click(object sender, RoutedEventArgs e)
        {
            var mapView = MapView.Active;
            if (mapView == null)
                return;

            var featureLayers = mapView.Map.Layers.OfType<FeatureLayer>();
            var disLayer = featureLayers.ElementAt(cbo_change.SelectedIndex);

            var filter = new ArcGIS.Core.Data.QueryFilter();
            switch (txt_value.Text)
            {
                case "1":
                    filter.WhereClause = "OBJECTID = 1";
                    break;
                case "2":
                    filter.WhereClause = "OBJECTID = 2";
                    break;
                case "3":
                    filter.WhereClause = "OBJECTID = 3";
                    break;
                case "4":
                    filter.WhereClause = "OBJECTID = 4";
                    break;
                default:
                    filter.WhereClause = "OBJECTID = 4";
                    break;
            }


            await ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
            {
                var rows = disLayer.Search(filter);
                while (rows.MoveNext())
                {
                    ArcGIS.Core.Data.Feature feature = rows.Current as ArcGIS.Core.Data.Feature;
                    var featExt = feature.GetShape().Extent;
                    mapView.ZoomTo(featExt);
                }
            });
        }

        private void txt_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            btn_go.IsEnabled = true;
        }
    }
}
