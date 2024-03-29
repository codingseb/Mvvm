﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// Extended MultiBinding (Allow to make nested hierarchy of bindings and markupextensions)
    /// </summary>
    [ContentProperty(nameof(Children))]
    [ContentWrapper(typeof(MarkupExtension))]
    public class XMultiBinding
        : MarkupExtension
    {
        #region Constructor and ManageArgs

        public XMultiBinding()
        { }

        public XMultiBinding(MarkupExtension markup1) => Children.Add(markup1);

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2)
            : this(markup1) => Children.Add(markup2);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3)
            : this(markup1, markup2) => Children.Add(markup3);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4)
            : this(markup1, markup2, markup3) => Children.Add(markup4);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5)
            : this(markup1, markup2, markup3, markup4) => Children.Add(markup5);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6)
            : this(markup1, markup2, markup3, markup4, markup5) => Children.Add(markup6);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7)
            : this(markup1, markup2, markup3, markup4, markup5, markup6) => Children.Add(markup7);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7) => Children.Add(markup8);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8) => Children.Add(markup9);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9) => Children.Add(markup10);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10) => Children.Add(markup11);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11) => Children.Add(markup12);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12) => Children.Add(markup13);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13) => Children.Add(markup14);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14) => Children.Add(markup15);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15) => Children.Add(markup16);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16) => Children.Add(markup17);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17) => Children.Add(markup18);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18) => Children.Add(markup19);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19) => Children.Add(markup20);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20) => Children.Add(markup21);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21) => Children.Add(markup22);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22) => Children.Add(markup23);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23) => Children.Add(markup24);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24) => Children.Add(markup25);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25) => Children.Add(markup26);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26) => Children.Add(markup27);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27) => Children.Add(markup28);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28) => Children.Add(markup29);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29) => Children.Add(markup30);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30) => Children.Add(markup31);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31) => Children.Add(markup32);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32) => Children.Add(markup33);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33) => Children.Add(markup34);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34) => Children.Add(markup35);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35) => Children.Add(markup36);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36) => Children.Add(markup37);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37) => Children.Add(markup38);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38) => Children.Add(markup39);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39) => Children.Add(markup40);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40, MarkupExtension markup41)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39, markup40) => Children.Add(markup41);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40, MarkupExtension markup41, MarkupExtension markup42)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39, markup40, markup41) => Children.Add(markup42);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40, MarkupExtension markup41, MarkupExtension markup42, MarkupExtension markup43)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39, markup40, markup41, markup42) => Children.Add(markup43);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40, MarkupExtension markup41, MarkupExtension markup42, MarkupExtension markup43, MarkupExtension markup44)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39, markup40, markup41, markup42, markup43) => Children.Add(markup44);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40, MarkupExtension markup41, MarkupExtension markup42, MarkupExtension markup43, MarkupExtension markup44, MarkupExtension markup45)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39, markup40, markup41, markup42, markup43, markup44) => Children.Add(markup45);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40, MarkupExtension markup41, MarkupExtension markup42, MarkupExtension markup43, MarkupExtension markup44, MarkupExtension markup45, MarkupExtension markup46)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39, markup40, markup41, markup42, markup43, markup44, markup45) => Children.Add(markup46);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40, MarkupExtension markup41, MarkupExtension markup42, MarkupExtension markup43, MarkupExtension markup44, MarkupExtension markup45, MarkupExtension markup46, MarkupExtension markup47)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39, markup40, markup41, markup42, markup43, markup44, markup45, markup46) => Children.Add(markup47);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40, MarkupExtension markup41, MarkupExtension markup42, MarkupExtension markup43, MarkupExtension markup44, MarkupExtension markup45, MarkupExtension markup46, MarkupExtension markup47, MarkupExtension markup48)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39, markup40, markup41, markup42, markup43, markup44, markup45, markup46, markup47) => Children.Add(markup48);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40, MarkupExtension markup41, MarkupExtension markup42, MarkupExtension markup43, MarkupExtension markup44, MarkupExtension markup45, MarkupExtension markup46, MarkupExtension markup47, MarkupExtension markup48, MarkupExtension markup49)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39, markup40, markup41, markup42, markup43, markup44, markup45, markup46, markup47, markup48) => Children.Add(markup49);
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10, MarkupExtension markup11, MarkupExtension markup12, MarkupExtension markup13, MarkupExtension markup14, MarkupExtension markup15, MarkupExtension markup16, MarkupExtension markup17, MarkupExtension markup18, MarkupExtension markup19, MarkupExtension markup20, MarkupExtension markup21, MarkupExtension markup22, MarkupExtension markup23, MarkupExtension markup24, MarkupExtension markup25, MarkupExtension markup26, MarkupExtension markup27, MarkupExtension markup28, MarkupExtension markup29, MarkupExtension markup30, MarkupExtension markup31, MarkupExtension markup32, MarkupExtension markup33, MarkupExtension markup34, MarkupExtension markup35, MarkupExtension markup36, MarkupExtension markup37, MarkupExtension markup38, MarkupExtension markup39, MarkupExtension markup40, MarkupExtension markup41, MarkupExtension markup42, MarkupExtension markup43, MarkupExtension markup44, MarkupExtension markup45, MarkupExtension markup46, MarkupExtension markup47, MarkupExtension markup48, MarkupExtension markup49, MarkupExtension markup50)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9, markup10, markup11, markup12, markup13, markup14, markup15, markup16, markup17, markup18, markup19, markup20, markup21, markup22, markup23, markup24, markup25, markup26, markup27, markup28, markup29, markup30, markup31, markup32, markup33, markup34, markup35, markup36, markup37, markup38, markup39, markup40, markup41, markup42, markup43, markup44, markup45, markup46, markup47, markup48, markup49) => Children.Add(markup50);

        #endregion

        /// <summary>
        /// Direct children of the XMultiBinding
        /// </summary>
        public Collection<MarkupExtension> Children { get; } = new Collection<MarkupExtension>();

        /// <summary>
        /// Gets or sets the converter to use to convert the source values to or from the target value.
        /// </summary>
        public IMultiValueConverter Converter { get; set; }

        /// <summary>
        /// Gets or sets an optional parameter to pass to a converter as additional information.
        /// </summary>
        public object ConverterParameter { get; set; }

        /// <summary>
        /// Gets or sets the CultureInfo object that applies to any converter assigned to Children wrapped by the XMultiBinding or on the XMultiBinding itself
        /// </summary>
        public CultureInfo ConverterCultureInfo { get; set; }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider, false);
        }

        /// <summary>
        /// Special ProvideValue that can indicate that this MarkupExtension is used in a other XMultibinding for example and is not the root binding
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension</param>
        /// <param name="hierarchyBuilding">Should be given as <c>true</c> when this markup extension is used in an other XMultibinding. So it do not set the binding directly on the dependencyProperty</param>
        /// <returns>A MultiBinding with all the hierarchy logic</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public object ProvideValue(IServiceProvider serviceProvider, bool hierarchyBuilding)
        {
            if (Converter == null)
                throw new ArgumentNullException(nameof(Converter));

            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service)
                || !(service.TargetObject is DependencyObject targetObject)
                || !(service.TargetProperty is DependencyProperty targetProperty))
            {
                return this;
            }

            var internalConverter = new InternalConverter()
            {
                Converter = Converter,
                ConverterParameter = ConverterParameter,
                ConverterCultureInfo = ConverterCultureInfo,
                IsInHierarchy = hierarchyBuilding,
                TargetProperty = targetProperty
            };

            var multiBinding = new MultiBinding()
            {
                Converter = internalConverter
            };

            internalConverter.Children = Children.Select(markup =>
            {
                if (markup is XMultiBinding xMultiBinding && xMultiBinding.ProvideValue(serviceProvider, true) is MultiBinding subMultiBindingFromX)
                {
                    subMultiBindingFromX.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
                    return subMultiBindingFromX;
                }
                else if (markup is MultiBinding subMultiBinding)
                {
                    subMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
                    return subMultiBinding;
                }
                else if (markup is BindingBase bindingBase)
                {
                    multiBinding.Bindings.Add(bindingBase);
                    return bindingBase;
                }
                else
                {
                    object providedValue = null;
                    MethodInfo methodInfo = markup.GetType().GetMethod("ProvideValue", new Type[] { typeof(IServiceProvider), typeof(bool) });

                    if (methodInfo != null)
                    {
                        providedValue = methodInfo.Invoke(markup, new object[] { serviceProvider, true } );
                    }
                    else
                    {
                        providedValue = markup.ProvideValue(serviceProvider);
                    }

                    if (providedValue is MultiBinding providedMultiBinding)
                    {
                        providedMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
                        return providedMultiBinding;
                    }
                    else if ((bindingBase = providedValue as Binding) == null)
                    {
                        bindingBase = new Binding()
                        {
                            Source = providedValue,
                            Mode = BindingMode.OneWay
                        };
                    }

                    multiBinding.Bindings.Add(bindingBase);
                    return bindingBase;
                }
            }).ToList();

            if (hierarchyBuilding)
            {
                return multiBinding;
            }
            else
            {
                BindingOperations.SetBinding(targetObject, targetProperty, multiBinding);

                return multiBinding.ProvideValue(serviceProvider);
            }
        }

        protected class InternalConverter : IMultiValueConverter
        {
            public IMultiValueConverter Converter { get; set; }
            public object ConverterParameter { get; set; }
            public CultureInfo ConverterCultureInfo { get; set; }
            public List<BindingBase> Children { get; set; }
            public bool IsInHierarchy { get; set; }
            public DependencyProperty TargetProperty { get; set; }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                List<object> groupedBindingsResults = new List<object>();

                int offset = 0;

                Children.ForEach(bindingBase =>
                {
                    if (bindingBase is MultiBinding multiBinding)
                    {
                        groupedBindingsResults.Add(multiBinding.Converter.Convert(values.Skip(offset).Take(multiBinding.Bindings.Count).ToArray(), null, multiBinding.ConverterParameter, multiBinding.ConverterCulture));
                        offset += multiBinding.Bindings.Count;
                    }
                    else
                    {
                        groupedBindingsResults.Add(values[offset]);
                        offset++;
                    }
                });

                object result = Converter.Convert(groupedBindingsResults.ToArray(), null, ConverterParameter, ConverterCultureInfo);

                if (!IsInHierarchy)
                {
                    result = MarkupStandardTypeConverter.ConvertValueForDependencyProperty(result, TargetProperty);
                }

                return result;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }
    }
}
