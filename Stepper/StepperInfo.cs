﻿using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Stepper
{
    public class StepperInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Stepper";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("370f4cbc-6fc8-4536-aac1-75da372fd294");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
