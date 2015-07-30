using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;
using Microsoft.SolverFoundation.Services;
using System.Text.RegularExpressions;

namespace CsGoTrader
{
    public static class OptimizationEngine
    {
        static void Main(string[] args)
        {
            //Sample1();
        }

        public static TradeUpContract checkCollection(Collection collection)
        {
            return findBestContract(
                collection.jsonSkins.Where(s => s.collectionGrade == CollectionGrade.Covert).ToList(),
                collection.jsonSkins.Where(s => s.collectionGrade == CollectionGrade.Classified).ToList());
            //return null;
        }

        static void Sample1()
        {
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            Decision x1 = new Decision(Domain.IntegerRange(0, 10), "x1");
            Decision x2 = new Decision(Domain.IntegerRange(0, 10), "x2");
            Decision x3 = new Decision(Domain.IntegerRange(0, 10), "x3");

            Decision t1fn = new Decision(Domain.IntegerRange(0, 1), "t1fn");
            Decision t1mw = new Decision(Domain.IntegerRange(0, 1), "t1mw");
            Decision t1ft = new Decision(Domain.IntegerRange(0, 1), "t1ft");

            Decision t2fn = new Decision(Domain.IntegerRange(0, 1), "t2fn");
            Decision t2mw = new Decision(Domain.IntegerRange(0, 1), "t2mw");
            Decision t2ft = new Decision(Domain.IntegerRange(0, 1), "t2ft");



            Rational M = 100;

            //model.AddDecisions(x1, x2);
            model.AddDecisions(x1, x2, x3, t1fn, t1mw, t1ft, t2fn, t2mw, t2ft);

            model.AddConstraint("Row0", x1 + x2 + x3 == 10);
            model.AddConstraint("Quality1", t1fn + t1mw + t1ft == 1);
            model.AddConstraint("Quality2", t2fn + t2mw + t2ft == 1);

            //string fnTerm = ""
            //model.AddConstraint("Row1", (x1 * 0.035 + x2 * 0.11) / 10 <= 0.07);

            //target 1
            model.AddConstraint("Target1FN", " t1fn - 1 + ((x1 * 0.035 + x2 * 0.11 + x3 * 0.25) / 10) <= 0.07");
            model.AddConstraint("Target1MW", " t1mw - 1 + ((x1 * 0.035 + x2 * 0.11 + x3 * 0.25) / 10) <= 0.15");
            model.AddConstraint("Target1FT", " t1ft - 1 + ((x1 * 0.035 + x2 * 0.11 + x3 * 0.25) / 10) <= 0.35");

            //target 2
            model.AddConstraint("Target2FN", " t2fn - 1 + ((x1 * 0.035 + x2 * 0.11 + x3 * 0.25) / 10) * 0.72 + 0.06 <= 0.07");
            model.AddConstraint("Target2MW", " t2mw - 1 + ((x1 * 0.035 + x2 * 0.11 + x3 * 0.25) / 10) * 0.72 + 0.06 <= 0.15");
            model.AddConstraint("Target2FT", " t2ft - 1 + ((x1 * 0.035 + x2 * 0.11 + x3 * 0.25) / 10) * 0.72 + 0.06 <= 0.35");

            //model.AddConstraint("Row2", mw * (x1 * 0.035 + x2 * 0.11) / 10 <= 0.15);

            //model.AddConstraint("Row2", x2 + z * 100 <= 100);

            Goal goal = model.AddGoal("Goal0", GoalKind.Maximize, (t1fn * 120 + t1mw * 61 + t1ft * 34 + t2fn * 76 + t2mw * 61 + t2ft * 34) - (x1 * 9 + x2 * 8 + x3 * 5) * 2);

            //Goal goal = model.AddGoal("Goal0", GoalKind.Maximize,
            //    "Max[x1-0.07,0] * 4 - x1");

            SimplexDirective directive = new SimplexDirective();
            directive.Arithmetic = Arithmetic.Exact;
            Solution solution = context.Solve(directive);
            Report report = solution.GetReport();
            Console.WriteLine("x1: {0}, x: {1}", x1, x2, x3);
            Console.WriteLine("Gain: {0}", goal);

            Console.Write("{0}", report);
            context.ClearModel();
        }

        public static TradeUpContract findBestContract(List<Skin> higherTier, List<Skin> lowerTier)
        {
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();
            var targets = getDecisions(higherTier, 1);
            foreach(var decision in targets)
            {
                model.AddDecision(decision.Item1);
            }

            var decisions = getDecisions(lowerTier, 10);
            foreach (var decision in decisions)
            {
                model.AddDecision(decision.Item1);
            }

            List<string> basicConstraints = new List<string>();
            foreach (var target in higherTier)
            {
                basicConstraints.Add(getBasicConstraints(target, 1));
            }

            basicConstraints.Add(getBasicConstraints(lowerTier, 10));

            int i = 0;
            foreach(var constraint in basicConstraints)
            {
                model.AddConstraint("basicConstraint" + i, constraint);
                i++;
            }

            List<string> complexConstraintsBase = getComplexConstraintsBase(lowerTier);

            List<string> complexConstraints = new List<string>();
            foreach(var target in targets)
            {
                var constraintTemplate = "{0} - 1 + (({1}) / 10) * {2} + {3} <= {4}";
                complexConstraints.Add(
                    string.Format(
                        constraintTemplate,
                        getVariableName(target.Item2, target.Item3),
                        string.Join(" + ",complexConstraintsBase),
                        target.Item2.getFloatValueRange(),
                        target.Item2.minFloatValue,
                        EnumUtil.getQualityBorder(target.Item3)));
            }

            i = 0;
            foreach(var constraint in complexConstraints)
            {
                model.AddConstraint("ComplexConstraint"+i, constraint);
                i++;
            }

            string goal = generateGoal(targets, decisions);

            model.AddGoal("mainGoal", GoalKind.Maximize, goal);

            SimplexDirective directive = new SimplexDirective();
            directive.Arithmetic = Arithmetic.Exact;
            Solution solution = context.Solve(directive);
            Report report = solution.GetReport();
            //Console.WriteLine("x1: {0}, x: {1}", x1, x2, x3);
            //Console.WriteLine("Gain: {0}", goal);

            Console.Write("{0}", report);
            context.ClearModel();

            return null;
        }

        private static string generateGoal(List<Tuple<Decision, Skin, Quality>> targets, List<Tuple<Decision, Skin, Quality>> decisions)
        {
            string goalTemplate = "(({0}) - ({1}) * {2}) * 0.85";
            string gain = generateGain(targets);
            string expenses = generateCosts(decisions);

            return string.Format(goalTemplate, gain, expenses, (targets.Count / 5));
        }

        private static string generateCosts(List<Tuple<Decision, Skin, Quality>> decisions)
        {
            List<string> expenses = new List<string>();
            foreach (var decision in decisions)
            {
                expenses.Add(getVariableName(decision.Item2, decision.Item3) + " * " + decision.Item2.averagePrice(decision.Item3, 10));
            }

            return string.Join(" + ", expenses);
        }

        private static string generateGain(List<Tuple<Decision, Skin, Quality>> targets)
        {
            List<string> expenses = new List<string>();
            foreach(var target in targets)
            {
                expenses.Add(getVariableName(target.Item2, target.Item3) + " * " + target.Item2.averagePrice(target.Item3, 1));
            }

            return string.Join(" + ", expenses);
        }

        private static List<string> getComplexConstraintsBase(List<Skin> lowerTier)
        {
            var complexConstraintsBase = new List<string>();

            foreach (var component in lowerTier)
            {
                foreach (Quality quality in Enum.GetValues(typeof(Quality)))
                {
                    complexConstraintsBase.Add(getVariableName(component, quality) + " * " + component.getAverageFloatValue(quality));
                }
            }

            return complexConstraintsBase;
        }

        public static string getVariableName(Skin skin, Quality quality)
        {
            var name = String.Copy(skin.name);
            var charsToRemove = new string[] { "|", "-", " ", "}", ",", ".", ";", "'" };
            foreach (var c in charsToRemove)
            {
                name = name.Replace(c, string.Empty);
            }

            return name + EnumUtil.getQualityShort(quality);
        }

        //public static string getGainString(List<Skin> targetSkins)
        //{
        //    var gainList = new List<string>();
        //    foreach (var skin in targetSkins)
        //    {
        //        gainList.Add(getVariableName(skin))
        //    }
        //}

        public static List<Tuple<Decision, Skin, Quality>> getDecisions(List<Skin> skins, int limit)
        {
            var decisionsList = new List<Tuple<Decision, Skin, Quality>>();
            foreach(Skin skin in skins)
            {
                foreach (Quality quality in Enum.GetValues(typeof(Quality)))
                {
                    var decision = new Decision(Domain.IntegerRange(0, limit), getVariableName(skin, quality));
                    decisionsList.Add(new Tuple<Decision, Skin, Quality>(decision, skin, quality));
                }
            }

            return decisionsList;
        }

        public static string getBasicConstraints(List<Skin> skins, int limit)
        {
            var constraintsList = new List<string>();
            foreach (Skin skin in skins)
            {
                foreach (Quality quality in Enum.GetValues(typeof(Quality)))
                {
                    constraintsList.Add(getVariableName(skin, quality));
                }
            }

            string left = String.Join("+ ", constraintsList.ToArray());
            left = left + " == " + limit;

            return left;
        }

        public static string getBasicConstraints(Skin skin, int limit)
        {
            var list = new List<Skin>()
            {
                skin
            };
            return getBasicConstraints(list, limit);
        }
    }
}
