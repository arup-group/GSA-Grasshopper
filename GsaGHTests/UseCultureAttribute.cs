﻿using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Xunit.Sdk;

/// <summary>
///   Apply this attribute to your test method to replace the
///   <see cref="Thread.CurrentThread" /> <see cref="CultureInfo.CurrentCulture" /> and
///   <see cref="CultureInfo.CurrentUICulture" /> with another culture.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
  AllowMultiple = false,
  Inherited = true)]
public class UseCultureAttribute : BeforeAfterTestAttribute {

  #region Properties + Fields
  /// <summary>
  ///   Gets the culture.
  /// </summary>
  public CultureInfo Culture => _culture.Value;

  /// <summary>
  ///   Gets the UI culture.
  /// </summary>
  public CultureInfo UiCulture => _uiCulture.Value;

  private readonly Lazy<CultureInfo> _culture;
  private readonly Lazy<CultureInfo> _uiCulture;
  private CultureInfo _originalCulture;
  private CultureInfo _originalUiCulture;
  #endregion Properties + Fields

  #region Public Constructors
  /// <summary>
  ///   Replaces the culture and UI culture of the current thread with
  ///   <paramref name="culture" />
  /// </summary>
  /// <param name="culture">The name of the culture.</param>
  /// <remarks>
  ///   <para>
  ///     This constructor overload uses <paramref name="culture" /> for both
  ///     <see cref="Culture" /> and <see cref="UiCulture" />.
  ///   </para>
  /// </remarks>
  public UseCultureAttribute(string culture) : this(culture, culture) { }

  /// <summary>
  ///   Replaces the culture and UI culture of the current thread with
  ///   <paramref name="culture" /> and <paramref name="uiCulture" />
  /// </summary>
  /// <param name="culture">The name of the culture.</param>
  /// <param name="uiCulture">The name of the UI culture.</param>
  public UseCultureAttribute(string culture, string uiCulture) {
    _culture = new Lazy<CultureInfo>(() => new CultureInfo(culture, false));
    _uiCulture = new Lazy<CultureInfo>(() => new CultureInfo(uiCulture, false));
  }

  #endregion Public Constructors

  #region Public Methods
  /// <summary>
  ///   Restores the original <see cref="CultureInfo.CurrentCulture" /> and
  ///   <see cref="CultureInfo.CurrentUICulture" /> to <see cref="Thread.CurrentPrincipal" />
  /// </summary>
  /// <param name="methodUnderTest">The method under test</param>
  public override void After(MethodInfo methodUnderTest) {
    Thread.CurrentThread.CurrentCulture = _originalCulture;
    Thread.CurrentThread.CurrentUICulture = _originalUiCulture;

    CultureInfo.CurrentCulture.ClearCachedData();
    CultureInfo.CurrentUICulture.ClearCachedData();
  }

  /// <summary>
  ///   Stores the current <see cref="Thread.CurrentPrincipal" />
  ///   <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" />
  ///   and replaces them with the new cultures defined in the constructor.
  /// </summary>
  /// <param name="methodUnderTest">The method under test</param>
  public override void Before(MethodInfo methodUnderTest) {
    _originalCulture = Thread.CurrentThread.CurrentCulture;
    _originalUiCulture = Thread.CurrentThread.CurrentUICulture;

    Thread.CurrentThread.CurrentCulture = Culture;
    Thread.CurrentThread.CurrentUICulture = UiCulture;

    CultureInfo.CurrentCulture.ClearCachedData();
    CultureInfo.CurrentUICulture.ClearCachedData();
  }

  #endregion Public Methods
}
