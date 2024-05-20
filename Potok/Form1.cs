using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;


namespace Potok
{
    public partial class Form1 : Form
    {
        private bool calculating = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null; // Очистка старого изображения

            if (!calculating)
            {
                calculating = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(CalculateMandelbrot));// Запускаем асинхронное вычисление фрактала Мандельброта с помощью пула потоков

            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null; // Очистка старого изображения

            if (!calculating)//// Проверяем, не происходит ли уже вычисление
            {
                calculating = true;//// Устанавливаем флаг, что началось вычисление
                await Task.Run(() => CalculateMandelbrot(null));//// Запускаем асинхронную задачу для вычисления фрактала Мандельброта
            }
        }

        //алгоритм построения фрактала Мандельброта
        private void CalculateMandelbrot(object state)
        {
            // Очистка старого изображения
            pictureBox1.Image = null;
            //Задаются ширина и высота изображения, создается новый Bitmap для рисования.
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;
            Bitmap bmp = new Bitmap(width, height);
            double zoom = 1.0;
            double moveX = -0.5; // Смещение по X
            double moveY = 0.0; // Смещение по Y

            //Происходит цикл по строкам изображения (по координате y).
            for (int y = 0; y < height; y++)
            {
                if (!calculating)
                {
                    break;
                }

                //Для каждой строки вычисляется комплексное число cy в зависимости от текущей координаты y.
                double cy = (y - height / 2.0) / (0.5 * zoom * height) + moveY;

                //Внутренний цикл идет по каждому пикселю в строке (по координате x).
                for (int x = 0; x < width; x++)
                {
                    //Для каждого пикселя вычисляется комплексное число cx в зависимости от текущей координаты x.
                    double cx = (x - width / 2.0) / (0.5 * zoom * width) + moveX;
                    double zx = 0;//представляет собой реальную часть числа
                    double zy = 0; //мнимую часть числа
                    int iteration = 0;
                    int maxIterations = 100000; // увеличиваем количество итераций

                    // Выполняется итеративный процесс вычисления цвета для точки путем проверок и преобразований по формуле Мандельброта.
                    while (zx * zx + zy * zy < 4 && iteration < maxIterations) //проверяется условие ограничения - модуль квадратного комплексного числа меньше 4, и количество итераций меньше максимального значения.
                    {
                        double zxtemp = zx * zx - zy * zy + cx;//Рассчитывается новое значение реальной части числа 
                        zy = 2.0 * zx * zy + cy;//Рассчитывается новое значение мнимой части числа
                        zx = zxtemp;
                        iteration++;
                    }

                    //Если точка принадлежит множеству Мандельброта (итерация меньше максимального числа итераций), устанавливается цвет в зависимости от числа итераций.
                    if (iteration < maxIterations)
                    {
                        // Цвет точки (x, y) в зависимости от числа итераций
                        Color color = Color.FromArgb((iteration % 256), 0, 0);
                        bmp.SetPixel(x, y, color);
                    }
                    else
                    {
                        bmp.SetPixel(x, y, Color.Black);//В противном случае точке устанавливается черный цвет.
                    }
                }

                UpdateProgress((int)((double)y / height * 100)); // Обновление прогресса вычислений
            }

            pictureBox1.Image = bmp; // Отображение изображения
            calculating = false;//Переменная calculating устанавливается в false, чтобы указать, что расчеты завершены.
        }



        private void UpdateProgress(int value)
        {
            // Проверяем, нужно ли вызывать метод из другого потока
            if (InvokeRequired)
            {
                //// Если да, вызываем метод UpdateProgress асинхронно с помощью BeginInvoke
                BeginInvoke(new Action<int>(UpdateProgress), value);
                return;
            }
            //// Обновляем значение прогресс-бара progressBar1
            progressBar1.Value = value;
        }

      

        private void button2_Click(object sender, EventArgs e)
        {
            calculating = false;// Устанавливаем флаг calculating в false при нажатии на кнопку button2
        }

        
    }
}
