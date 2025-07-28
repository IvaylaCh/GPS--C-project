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
    /*public partial class Form1 : Form
    {
        string[] cities;
        private int[,] graph;
        string fileInfo = string.Empty;
        public Form1()
        {
            InitializeComponent();

        }

        public void WriteCities(string fileDirectory)

        {
            using (StreamReader reader = new StreamReader(fileDirectory, Encoding.UTF8))
            {
                StringBuilder sb = new StringBuilder();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
                fileInfo = sb.ToString().TrimEnd();
            }
        }
        public void FillUpComboBoxes()
        {
            for (int i = 0; i < cities.Length && cities[i] != null; i++)
            {
                comboBox1.Items.Add(cities[i]);
                comboBox2.Items.Add(cities[i]);
            }
        }


        public void Form1_Load(object sender, EventArgs e)
        {
            WriteCities("path.txt");
            ParseCitiesAndDistances();
            FillUpComboBoxes();
        }
        private void ParseCitiesAndDistances()
        {
            string[] fileInfoSeparatedByNewLine = fileInfo.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            cities = new string[fileInfoSeparatedByNewLine.Length * 2];
            graph = new int[cities.Length, cities.Length];

            int index = 0;

            for (int i = 0; i < fileInfoSeparatedByNewLine.Length; i++)
            {
                string[] citiesAndDistance = fileInfoSeparatedByNewLine[i].Split(' ');
                string city1 = citiesAndDistance[0];
                string city2 = citiesAndDistance[1];

                if (!string.IsNullOrEmpty(city1) && !Array.Exists(cities, element => element == city1))
                    cities[index++] = city1;

                if (!string.IsNullOrEmpty(city2) && !Array.Exists(cities, element => element == city2))
                    cities[index++] = city2;

                int city1Index = Array.IndexOf(cities, city1);
                int city2Index = Array.IndexOf(cities, city2);
                int distance = int.Parse(citiesAndDistance[2]);

                graph[city1Index, city2Index] = distance;
                graph[city2Index, city1Index] = distance;
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            comboBox2.Items.Clear();
            for (int i = 0; i < cities.Length && cities[i] != null; i++)
            {
                comboBox2.Items.Add(cities[i]);
            }
            comboBox2.Items.Remove(comboBox1.Text);
            if (comboBox2.Text != "") 
            FindTheShortestDistance(comboBox1.Text, comboBox2.Text);

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            comboBox1.Items.Clear();
            for (int i = 0; i < cities.Length && cities[i] != null; i++)
            {
                comboBox1.Items.Add(cities[i]);
            }
            comboBox1.Items.Remove(comboBox2.Text);
            if (comboBox1.Text != "") 
            FindTheShortestDistance(comboBox1.Text, comboBox2.Text);
        }

        private void FindTheShortestDistance(string firstCity, string secondCity)
        {
            bool[] used = new bool[cities.Length];
            int[] distances = new int[cities.Length];
            int[] parents = new int[cities.Length];
            int[,] graph = new int[cities.Length, cities.Length];
            int startIndex = Array.IndexOf(cities,firstCity);
            int endIndex = Array.IndexOf(cities, secondCity);
            List<string> fileInfoSeparatedByNewLine = fileInfo.Split(new string[] { Environment.NewLine },
    StringSplitOptions.None).ToList();
            //algorithm start
            for (int i = 0; i < fileInfoSeparatedByNewLine.Count; i++)
            {
                string[] citiesAndDistance = fileInfoSeparatedByNewLine[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int city1 = int.Parse(citiesAndDistance[0]) - 1;
                int city2 = int.Parse(citiesAndDistance[1]) - 1;
                int distance = int.Parse(citiesAndDistance[2]);
                graph[city1, city2] = distance;
                graph[city2, city1] = distance;
            }
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = int.MaxValue;
                parents[i] = -1;
            }
            distances[startIndex] = 0;
            int currentIndex = startIndex;
            while (!used[currentIndex])
            {
                for (int i = 0; i < graph.GetLength(0); i++)
                {
                    if (graph[currentIndex, i] != 0)
                    {
                        int dist = graph[currentIndex, i] + distances[currentIndex];
                        if (dist < distances[i])
                        {
                            distances[i] = dist;
                            parents[i] = currentIndex;
                        }
                    }
                }
                used[currentIndex] = true;
                for (int i = 0; i < graph.GetLength(0); i++)
                {
                    if (distances[i] < int.MaxValue && !used[i])
                    {
                        currentIndex = i;
                        break;
                    }
                }
            }
            List<int> shortestPath = new List<int>();
            //algorithm end
            int current = endIndex;
            while (current != startIndex)
            {
                shortestPath.Add(current);
                current = parents[current];
            }
           // shortestPath.Add(Array.IndexOf());
            shortestPath.Reverse();
            for (int i = 0; i < shortestPath.Count; i++)
            {
                listBox1.Items.Add(cities[shortestPath[i]]);
            }
            textBox1.Text = distances[endIndex].ToString();

        }
    }*/

    


    




    



