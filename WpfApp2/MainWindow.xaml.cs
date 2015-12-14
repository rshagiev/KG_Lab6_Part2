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
using System.Windows.Media.Media3D;

namespace WpfApp2
{

    public partial class MainWindow : Window
    {
        private GeometryModel3D mGeometry;//геометрическая модель, которую будем рисовать
        private bool mDown;
        private Point mLastPos;

        public MainWindow()
        {
            InitializeComponent();
            BuildSolid();//вызов функции, которая будет рисовать
        }
        private void BuildSolid()
        {
            float phi = (float)(Math.Sqrt(5) + 1) / 2;
            MeshGeometry3D mesh = new MeshGeometry3D();//создаем сетку, на основе которой будет создана модель
            mesh.Positions.Add(new Point3D(0, 1 / phi, phi));       //0
            mesh.Positions.Add(new Point3D(-1, 1, 1));              //1
            mesh.Positions.Add(new Point3D(-phi, 0, 1 / phi));      //2
            mesh.Positions.Add(new Point3D(-1, -1, 1));             //3

            mesh.Positions.Add(new Point3D(0, -1 / phi, phi));      //4
            mesh.Positions.Add(new Point3D(-1 / phi, -phi, 0));     //5
            mesh.Positions.Add(new Point3D(1 / phi, -phi, 0));      //6
            mesh.Positions.Add(new Point3D(1, -1, 1));              //7
            mesh.Positions.Add(new Point3D(phi, 0, 1 / phi));       //8
            mesh.Positions.Add(new Point3D(1, 1, 1));               //9
            mesh.Positions.Add(new Point3D(-1 / phi, phi, 0));      //10
            mesh.Positions.Add(new Point3D(1 / phi, phi, 0));       //11
            mesh.Positions.Add(new Point3D(-1, -1, -1));            //12
            mesh.Positions.Add(new Point3D(-phi, 0, -1 / phi));     //13
            mesh.Positions.Add(new Point3D(-1, 1, -1));             //14
            mesh.Positions.Add(new Point3D(phi, 0, -1 / phi));      //15
            mesh.Positions.Add(new Point3D(1, -1, -1));             //16
            mesh.Positions.Add(new Point3D(0, -1 / phi, -phi));     //17
            mesh.Positions.Add(new Point3D(1, 1, -1));              //18
            mesh.Positions.Add(new Point3D(0, 1 / phi, -phi));      //19

            //массив для отрисовки
            int[,] faces;
            faces = new int[,] // геометрия фигуры
            {
                {0, 1, 2, 3, 4},                                    //0
                {3, 5, 6, 7, 4},                                    //1
                {4, 7, 8, 9, 0},                                    //2

                {0, 9, 11, 10, 1},                                  //3
                {3, 2, 13, 12, 5},                                  //4
                {2, 1, 10, 14, 13},                                 //5

                {16, 15, 8, 7, 6},                                  //6
                {6, 5, 12, 17, 16},                                 //7
                {15, 18, 11, 9, 8},                                 //8

                {17, 19, 18, 15, 16},                               //9
                {19, 17, 12, 13, 14},                               //10
                {10, 11, 18, 19, 14}                                //11
            };
            for (int num = 0; num < 12; num++)
            {
                mesh.TriangleIndices.Add(faces[num, 0]); mesh.TriangleIndices.Add(faces[num, 1]); mesh.TriangleIndices.Add(faces[num, 3]);
                mesh.TriangleIndices.Add(faces[num, 1]); mesh.TriangleIndices.Add(faces[num, 2]); mesh.TriangleIndices.Add(faces[num, 3]);
                mesh.TriangleIndices.Add(faces[num, 0]); mesh.TriangleIndices.Add(faces[num, 3]); mesh.TriangleIndices.Add(faces[num, 4]);
                mGeometry = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.DarkViolet));//создаем модель из сетки
                mGeometry.Transform = new Transform3DGroup();//создаем трансформацию для модели
            }
            group.Children.Add(mGeometry);//группируем модели(для общего освещения, преобразования и тд.)
 
        }


        //отдаление камеры при прокрутке мыши
        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z - e.Delta / 500D);
        }

        //обрабатывание нажатия мыши      
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            mDown = true;
            Point pos = Mouse.GetPosition(viewport);
            mLastPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
        }

        //обрабатывает прекращение нажатия мыши
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mDown = false;
        }

        //вращение модели при нажатой левой кнопки мыши и передвижением мыши
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (mDown)
            {
                Point pos = Mouse.GetPosition(viewport);
                Point actualPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
                double dx = actualPos.X - mLastPos.X, dy = actualPos.Y - mLastPos.Y;

                double mouseAngle = 0;
                if (dx != 0 && dy != 0)
                {
                    mouseAngle = Math.Asin(Math.Abs(dy) / Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    if (dx < 0 && dy > 0)
                        mouseAngle += Math.PI / 2;
                    else if (dx < 0 && dy < 0)
                        mouseAngle += Math.PI;
                    else if (dx > 0 && dy < 0)
                        mouseAngle += Math.PI * 1.5;
                }
                else if (dx == 0 && dy != 0)
                    mouseAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
                else if (dx != 0 && dy == 0)
                    mouseAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;

                double axisAngle = mouseAngle + Math.PI / 2;

                Vector3D axis = new Vector3D(Math.Cos(axisAngle) * 4, Math.Sin(axisAngle) * 4, 0);

                double rotation = 0.01 * Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                Transform3DGroup group = mGeometry.Transform as Transform3DGroup;
                QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, rotation * 180 / Math.PI));
                group.Children.Add(new RotateTransform3D(r));
                mLastPos = actualPos;
            }
        }
    }
}
