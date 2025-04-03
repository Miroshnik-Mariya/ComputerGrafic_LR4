using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ComputerGrafics_LR4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap bitmap;
        System.Drawing.Image image;
        int[,] brightnessMatrix; //матрица яркостей

       


        private void ExportBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog();
            open_dialog.Filter = ("Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"); //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(open_dialog.FileName);
                    image = pictureBox1.Image;
                    Bitmap source = new Bitmap(pictureBox1.Image, pictureBox1.Width, pictureBox1.Height);
                    pictureBox1.Image = source;
                    bitmap = new Bitmap(image);
                    brightnessMatrix = new int[image.Height, image.Width];
                    FormImageMatrix();
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть выбранный файл");
                }
            }

        }




        //метод формирования матрицы яркостей
        private void FormImageMatrix() 
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    int r = Convert.ToInt32(c.R);
                    int b = Convert.ToInt32(c.B);
                    int g = Convert.ToInt32(c.G);
                    int brit = Convert.ToInt32(0.3 * r + 0.59 * g + 0.11 * b);
                    brightnessMatrix[y, x] = Convert.ToByte(brit);
                }
            }
        }



        //преобразование в оттенки серого 
        private Bitmap ConvertToGrey(Bitmap grid)
        {
            int w = grid.Width;
            int h = grid.Height;
            Color color;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    color = grid.GetPixel(x, y);
                    int avg = (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B);
                    grid.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                }
            }
            return grid;
        }

        /*
        private Bitmap LinearContrast(Bitmap grid)
        {
            int width = grid.Width;
            int height = grid.Height;
            int p1 = int.Parse(numericUpDown1.Text);
            int p2 = int.Parse(numericUpDown2.Text);

            if (p1>=1 && p2>=1 && p1<=100 && p2<=100) { }
            else 
            { }
            // Создаем массивы для хранения значений яркости пикселей
            int[] grayScaleValues = new int[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = grid.GetPixel(x, y);
                    int avg = (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B);
                    grayScaleValues[y * width + x] = avg;
                }
            }

            Array.Sort(grayScaleValues);

            // Вычисляем минимальное и максимальное значения, используя проценты 
            int minGray = grayScaleValues[(int)(p1 / 100.0 * (width * height - 1))];
            int maxGray = grayScaleValues[(int)(p2 / 100.0 * (width * height - 1))];

            minGray = Math.Max(0, Math.Min(255, minGray));
            maxGray = Math.Max(0, Math.Min(255, maxGray));

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color originalColor = grid.GetPixel(x, y);

                    float newRed = (float)(originalColor.R - minGray) / (maxGray - minGray) * 255;
                    float newGreen = (float)(originalColor.G - minGray) / (maxGray - minGray) * 255;
                    float newBlue = (float)(originalColor.B - minGray) / (maxGray - minGray) * 255;

                    newRed = Math.Max(0, Math.Min(255, newRed));
                    newGreen = Math.Max(0, Math.Min(255, newGreen));
                    newBlue = Math.Max(0, Math.Min(255, newBlue));

                    grid.SetPixel(x, y, Color.FromArgb((int)newRed, (int)newGreen, (int)newBlue));
                }
            }

            return grid;
        }
        */


        private Bitmap LinearContrast(Bitmap grid, double lowPercentile = 1.0, double highPercentile = 99.0)
        {
            int width = grid.Width;
            int height = grid.Height;

            int[] grayScaleValues = new int[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = grid.GetPixel(x, y);
                    int avg = (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B);
                    grayScaleValues[y * width + x] = avg;
                }
            }

            Array.Sort(grayScaleValues);
            int minGray = grayScaleValues[(int)(lowPercentile / 100.0 * (width * height - 1))];
            int maxGray = grayScaleValues[(int)(highPercentile / 100.0 * (width * height - 1))];

            minGray = Math.Max(0, Math.Min(255, minGray));
            maxGray = Math.Max(0, Math.Min(255, maxGray));

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color originalColor = grid.GetPixel(x, y);
                    float newRed = (float)(originalColor.R - minGray) / (maxGray - minGray) * 255;
                    float newGreen = (float)(originalColor.G - minGray) / (maxGray - minGray) * 255;
                    float newBlue = (float)(originalColor.B - minGray) / (maxGray - minGray) * 255;

                    newRed = Math.Max(0, Math.Min(255, newRed));
                    newGreen = Math.Max(0, Math.Min(255, newGreen));
                    newBlue = Math.Max(0, Math.Min(255, newBlue));

                    grid.SetPixel(x, y, Color.FromArgb((int)newRed, (int)newGreen, (int)newBlue));
                }
            }
            label9.Text = Convert.ToString(minGray);
            label10.Text = Convert.ToString(maxGray);
            return grid;
        }

        public static Bitmap HighPassFilter(Bitmap grid)
        {
            int width = grid.Width;
            int height = grid.Height;

            // Маска фильтра повышения контрастности
            int[,] filterMatrix = new int[,]
            {
            {-1, -1, -1},
            {-1,  9, -1},
            {-1, -1, -1}
            };

            // Создаем временную копию для хранения новых значений пикселей
            Color[,] tempPixels = new Color[width, height];

            // Обработка пикселей
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int newRed = 0, newGreen = 0, newBlue = 0;

                    // Применяем маску фильтра
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int neighborX = x + j;
                            int neighborY = y + i;

                            // Проверяем, что соседние пиксели находятся в пределах границ
                            if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                            {
                                // Получаем цвет соседнего пикселя
                                Color neighborPixel = grid.GetPixel(neighborX, neighborY);

                                // Применяем веса из маски
                                newBlue += neighborPixel.B * filterMatrix[i + 1, j + 1];
                                newGreen += neighborPixel.G * filterMatrix[i + 1, j + 1];
                                newRed += neighborPixel.R * filterMatrix[i + 1, j + 1];
                            }
                        }
                    }

                    // Ограничиваем значения в диапазоне 0-255
                    newRed = Math.Max(0, Math.Min(255, newRed));
                    newGreen = Math.Max(0, Math.Min(255, newGreen));
                    newBlue = Math.Max(0, Math.Min(255, newBlue));

                    // Сохраняем новое значение во временном массиве
                    tempPixels[x, y] = Color.FromArgb(newRed, newGreen, newBlue);
                }
            }

            // Применяем новые значения из временного массива к изображению
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    grid.SetPixel(x, y, tempPixels[x, y]);
                }
            }

            // Обработка краев: копируем значения с соседних пикселей
            // Верхняя и нижняя границы
            for (int x = 0; x < width; x++)
            {
                grid.SetPixel(x, 0, grid.GetPixel(x, 1));
                grid.SetPixel(x, height - 1, grid.GetPixel(x, height - 2));
            }

            // Левая и правая границы
            for (int y = 0; y < height; y++)
            {
                grid.SetPixel(0, y, grid.GetPixel(1, y));
                grid.SetPixel(width - 1, y, grid.GetPixel(width - 2, y));
            }

            return grid;
        }
    



        private void Grey_Btn_Click_1(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image, pictureBox1.Width, pictureBox1.Height);
            bmp = ConvertToGrey(bmp);
            pictureBox2.Image = bmp;

        }

        private void LinearContrast_btn_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox2.Image, pictureBox2.Width, pictureBox2.Height);
            bmp = LinearContrast(bmp);
            pictureBox3.Image = bmp;

        }

        private void Filter_btn_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox2.Image, pictureBox2.Width, pictureBox2.Height);
            bmp = HighPassFilter(bmp);
            pictureBox4.Image = bmp;
        }
    }
}

