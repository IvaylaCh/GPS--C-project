using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gps
{
    public partial class Form1 : Form
    {
        private const string FilePath = "path.txt"; // Path to the text file containing city distances

        private Dictionary<string, Dictionary<string, int>> graph;

        public Form1()
        {
            InitializeComponent();
            InitializeCities();
        }

        private void InitializeCities()
        {
            // Add cities to ComboBoxes
            string[] cities = { "София", "Пловдив", "Варна", "Бургас", "Русе", "Стара Загора", "Плевен", "Сливен", "Добрич", "Шумен", "Перник", "Хасково", "Ямбол", "Пазарджик", "Благоевград" };

            comboBox1.Items.AddRange(cities);
            comboBox2.Items.AddRange(cities);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCityDistances();
            MessageBox.Show("Първо избери град");
        }

        private void LoadCityDistances()
        {
            // Read city distances from the text file and populate the graph
            graph = new Dictionary<string, Dictionary<string, int>>();

            try
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        string city1 = parts[0];
                        string city2 = parts[1];
                        int distance = int.Parse(parts[2]);

                        if (!graph.ContainsKey(city1))
                            graph[city1] = new Dictionary<string, int>();

                        if (!graph.ContainsKey(city2))
                            graph[city2] = new Dictionary<string, int>();

                        graph[city1][city2] = distance;
                        graph[city2][city1] = distance;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Грешка: " + ex.Message, "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      

        private Dictionary<string, int> Dijkstra(Dictionary<string, Dictionary<string, int>> graph, string start)
        {
            Dictionary<string, int> distances = new Dictionary<string, int>();
            HashSet<string> visited = new HashSet<string>();

            foreach (string vertex in graph.Keys)
            {
                distances[vertex] = int.MaxValue;
            }

            distances[start] = 0;

            while (visited.Count < graph.Count)
            {
                string currentVertex = GetMinDistanceVertex(distances, visited);
                visited.Add(currentVertex);

                foreach (var neighbor in graph[currentVertex])
                {
                    int alternativeRoute = distances[currentVertex] + neighbor.Value;
                    if (alternativeRoute < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alternativeRoute;
                    }
                }
            }

            return distances;
        }

        private string GetMinDistanceVertex(Dictionary<string, int> distances, HashSet<string> visited)
        {
            return distances.Where(x => !visited.Contains(x.Key)).OrderBy(x => x.Value).First().Key;
        }

        private List<string> GetShortestPath(Dictionary<string, int> distances, string start, string end)
        {
            List<string> path = new List<string>();

            if (distances[end] == int.MaxValue)
                return null; // No path found

            string currentVertex = end;
            while (!currentVertex.Equals(start))
            {
                path.Insert(0, currentVertex);
                foreach (var neighbor in graph[currentVertex])
                {
                    if (distances[currentVertex] == distances[neighbor.Key] + neighbor.Value)
                    {
                        currentVertex = neighbor.Key;
                        break;
                    }
                }
            }

            path.Insert(0, start);
            return path;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string startCity = comboBox1.SelectedItem.ToString();
            string endCity = comboBox2.SelectedItem.ToString();

            if (graph.ContainsKey(startCity) && graph.ContainsKey(endCity))
            {
                Dictionary<string, int> distances = Dijkstra(graph, startCity);
                List<string> path = GetShortestPath(distances, startCity, endCity);

                if (path != null)
                {
                    int totalDistance = distances[endCity];
                    string pathString = string.Join(" - ", path);

                    textBox1.Text = $"{totalDistance+ " км."}";
                    textBox2.Text=$" {pathString}";
                }
                else
                {
                    MessageBox.Show("Няма открита пътека.", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Няма избран град.", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
    

    


    




    



