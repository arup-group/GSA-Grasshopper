using System.Collections.Generic;

using Grasshopper.Kernel;

using GsaGH.Parameters.Results;

namespace GsaGH.Helpers.GH {
  internal static class SendMessage {
    public static void SendErrorsAndWarnings<T>(
      GH_Component component, IEnumerable<IList<IEntity1dQuantity<T>>> collection) where T : IResultItem {
      foreach (IList<IEntity1dQuantity<T>> entity1dQuantities in collection) {
        foreach (IEntity1dQuantity<T> entity1dQuantity in entity1dQuantities) {
          foreach (string error in entity1dQuantity.Errors) {
            component.AddRuntimeError(error);
          }

          foreach (string warning in entity1dQuantity.Warnings) {
            component.AddRuntimeWarning(warning);
          }
        }
      }
    }
  }
}
