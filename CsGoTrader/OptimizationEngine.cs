using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;
using Microsoft.SolverFoundation.Services;

namespace CsGoTrader
{
    public static class OptimizationEngine
    {
        static void Main(string[] args)
        {
            Sample1();
        }

        static void Sample1()
        {
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            Decision x1 = new Decision(Domain.IntegerRange(0, 10), "x1");
            Decision x2 = new Decision(Domain.IntegerRange(0, 10), "x2");
            Decision x3 = new Decision(Domain.IntegerRange(0, 10), "x3");

            Decision fn = new Decision(Domain.IntegerRange(0, 1), "fn");
            Decision mw = new Decision(Domain.IntegerRange(0, 1), "mw");
            Decision ft = new Decision(Domain.IntegerRange(0, 1), "ft");



            Rational M = 100;

            //model.AddDecisions(x1, x2);
            model.AddDecisions(x1, x2, x3, fn, mw, ft);

            model.AddConstraint("Row0", x1 + x2 + x3 == 10);
            model.AddConstraint("Quality", fn + mw + ft == 1);

            //string fnTerm = ""
            //model.AddConstraint("Row1", (x1 * 0.035 + x2 * 0.11) / 10 <= 0.07);
            model.AddConstraint("Row1", " fn - 1 + ((x1 * 0.035 + x2 * 0.11 + x3 * 0.25) / 10) <= 0.07");
            model.AddConstraint("Row2", " mw - 1 + ((x1 * 0.035 + x2 * 0.11 + x3 * 0.25) / 10) <= 0.15");
            model.AddConstraint("Row3", " ft - 1 + ((x1 * 0.035 + x2 * 0.11 + x3 * 0.25) / 10) <= 0.35");

            //model.AddConstraint("Row2", mw * (x1 * 0.035 + x2 * 0.11) / 10 <= 0.15);

            //model.AddConstraint("Row2", x2 + z * 100 <= 100);

            Goal goal = model.AddGoal("Goal0", GoalKind.Maximize, fn * 76 + mw * 61 + ft * 34 - (x1 * 9 + x2 * 6 + x3 * 5));

            //Goal goal = model.AddGoal("Goal0", GoalKind.Maximize,
            //    "Max[x1-0.07,0] * 4 - x1");

            SimplexDirective directive = new SimplexDirective();
            directive.Arithmetic = Arithmetic.Exact;
            Solution solution = context.Solve(directive);
            Report report = solution.GetReport();
            Console.WriteLine("x1: {0}, x: {1}", x1, x2, x3);
            Console.WriteLine("Gain: {0}", goal);

            //Console.Write("{0}", report);
            context.ClearModel();
        }
    }
}
