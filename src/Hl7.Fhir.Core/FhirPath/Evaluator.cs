﻿using Hl7.Fhir.Navigation;
using Hl7.Fhir.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hl7.Fhir.FhirPath
{
    public delegate EvaluationContext Evaluator(EvaluationContext c);

    // There is a special case around the entry point, where the type of the entry point can be represented, but is optional.
    // To illustrate this point, take the path
    //      telecom.where(use = 'work').value
    // This can be evaluated as an expression on a Patient resource, or other kind of resources. 
    // However, for natural human use, expressions are often prefixed with the name of the context in which they are used:
    //	    Patient.telecom.where(use = 'work').value
    // These 2 expressions have the same outcome, but when evaluating the second, the evaluation will only produce results when used on a Patient resource.

    public static class Eval
    {
        public static IEnumerable<FhirNavigationTree> Evaluate(this Evaluator evaluator, FhirNavigationTree instance)
        {
            var context = EvaluationContext.NewContext(null, new List<FhirNavigationTree> { instance });
            var resultContxt = evaluator(context);

            if (resultContxt.IsAtomic)
                throw new NotImplementedException("Need to find elegant solution here");
            else
                return resultContxt.Nodes;
        }

        public static Evaluator Chain(IEnumerable<Evaluator> evaluators)
        {
            return c =>
                evaluators.Aggregate(c, (result, next) => next(result));
        }

        public static Evaluator ChildrenMatchingName(string name)
        {
            return c =>
            {
                if (c.IsNodeSet)
                   return EvaluationContext.NewContext(c, c.Nodes.IsMatch(name));
                else
                   throw Error.InvalidOperation("Cannot navigate to children {0} on an expression that is not a node-set");
            };
        }

        public static Evaluator Constant(object value)
        {
            return c => EvaluationContext.NewContext(c, value);
        }

        //public static Evaluator<decimal> Add(this Evaluator<decimal> left, Evaluator<decimal> right)
        //{
        //    return c => Result.ForValue(left(c).Value + right(c).Value);
        //}

        //public static Evaluator<decimal> Sub(this Evaluator<decimal> left, Evaluator<decimal> right)
        //{
        //    return c => Result.ForValue(left(c).Value - right(c).Value);
        //}

        //public static Evaluator<decimal> Mul(this Evaluator<decimal> left, Evaluator<decimal> right)
        //{
        //    return c => Result.ForValue(left(c).Value * right(c).Value);
        //}

        //public static Evaluator<decimal> Div(this Evaluator<decimal> left, Evaluator<decimal> right)
        //{
        //    return c => Result.ForValue(left(c).Value / right(c).Value);
        //}

        //public static Evaluator<string> Add(this Evaluator<string> left, Evaluator<string> right)
        //{
        //    return c => Result.ForValue(left(c).Value + right(c).Value);
        //}
    }
}
