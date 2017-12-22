using Ciloci.Flee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphing3D
{
    class Plotting2D
    {
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plinit();
        
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plcol0(int icol0);
      
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plend();
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plsdev(string devname);
    
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int c_plsfnam(String fnam);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plline(int n, double[] x, double[] y);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plenv(double xmin, double xmax, double ymin, double ymax, int just, int axis);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_pllab(string xlabel, string ylabel, string tlabel);

        private double[] x = null;
        private double[] y = null;

        public void Paint2D(double xMin, double xMax, ListView list, String funtion2D)
        {
            double yMin = 10000, yMax = -10000;
            int NSIZE = (int)(xMax - xMin) * 100;
            x = new double[NSIZE];
            y = new double[NSIZE];

            double step = (double)(xMax - xMin) / (double)NSIZE;

            int i;
            for (i = 0; i < NSIZE; i++)
            {
                x[i] = xMin + (double)(i) * step;
                // Define the context of our expression
                ExpressionContext context = new ExpressionContext();
                // Allow the expression to use all static public methods of System.Math
                context.Imports.AddType(typeof(Math));
                // Define an int variable
                context.Variables["x"] = x[i];
                IGenericExpression<double> eGeneric = context.CompileGeneric<double>(funtion2D);
                // Evaluate the expressions
                y[i] = eGeneric.Evaluate();
                if (i % 2 == 0) loadListView(x[i], y[i], list);
                if (yMax < y[i]) yMax = y[i];
                if (yMin > y[i]) yMin = y[i];
            }

            c_plsfnam("demo2d");
            c_plsdev("svg");
            c_plinit();

            //c_pladv(1);
            c_plcol0(1);

            c_plenv(xMin, xMax, yMin, yMax, 0, 0);
            c_pllab("x", "y", "Do thi: y = " + funtion2D);

            // Plot the data that was prepared above.
            c_plline(NSIZE, x, y);

            // Close PLplot library
            c_plend();

        }

        private void loadListView(double x, double y, ListView list)
        {
            String[] row = { x.ToString(), y.ToString() };
            ListViewItem item = new ListViewItem(row);
            list.Items.Add(item);
        }
    }
}

