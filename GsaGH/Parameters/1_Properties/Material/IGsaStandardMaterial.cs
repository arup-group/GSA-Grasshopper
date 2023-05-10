using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public interface IGsaStandardMaterial<T> : IGsaMaterial{
    T StandardMaterial { get; }
    string SteelDesignCodeName { get; }
    string ConcreteDesignCodeName { get; }
  }
}
