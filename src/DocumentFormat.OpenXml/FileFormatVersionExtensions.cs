﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DocumentFormat.OpenXml.Packaging;
using System;

namespace DocumentFormat.OpenXml
{
    internal static class FileFormatVersionExtensions
    {
        public static FileFormatVersions[] AllVersions { get; } = new[]
        {
            FileFormatVersions.Office2007,
            FileFormatVersions.Office2010,
            FileFormatVersions.Office2013,
            FileFormatVersions.Office2016,
            FileFormatVersions.Office2019,
            FileFormatVersions.Office2021,
            FileFormatVersions.Microsoft365,
        };

        /// <summary>
        /// Determines whether the supplied version is within the known set of versions
        /// </summary>
        /// <param name="version">The version to check</param>
        /// <returns>True if a known version, otherwise false</returns>
        public static bool Any(this FileFormatVersions version)
        {
            return version == FileFormatVersions.Office2007
                || version == FileFormatVersions.Office2010
                || version == FileFormatVersions.Office2013
                || version == FileFormatVersions.Office2016
                || version == FileFormatVersions.Office2019
                || version == FileFormatVersions.Office2021
                || version == FileFormatVersions.Microsoft365;
        }

        /// <summary>
        /// Determines if the supplied version is valid for all versions
        /// </summary>
        /// <param name="version">The version to check</param>
        /// <returns>True if the version is all of the known versions, otherwise false</returns>
        public static bool All(this FileFormatVersions version)
        {
            const FileFormatVersions AllVersions =
                  FileFormatVersions.Office2007
                | FileFormatVersions.Office2010
                | FileFormatVersions.Office2013
                | FileFormatVersions.Office2016
                | FileFormatVersions.Office2019
                | FileFormatVersions.Office2021
                | FileFormatVersions.Microsoft365;

            return version == AllVersions;
        }

        /// <summary>
        /// Combines values for the given version and all versions that come after it
        /// </summary>
        /// <param name="version">Version to which all other versions are added</param>
        /// <returns>A version instance with <paramref name="version"/> and all later versions</returns>
        public static FileFormatVersions AndLater(this FileFormatVersions version)
            => version switch
            {
                FileFormatVersions.Office2007 => FileFormatVersions.Office2007
                                                | FileFormatVersions.Office2010
                                                | FileFormatVersions.Office2013
                                                | FileFormatVersions.Office2016
                                                | FileFormatVersions.Office2019
                                                | FileFormatVersions.Office2021
                                                | FileFormatVersions.Microsoft365,
                FileFormatVersions.Office2010 => FileFormatVersions.Office2010
                                                | FileFormatVersions.Office2013
                                                | FileFormatVersions.Office2016
                                                | FileFormatVersions.Office2019
                                                | FileFormatVersions.Office2021
                                                | FileFormatVersions.Microsoft365,
                FileFormatVersions.Office2013 => FileFormatVersions.Office2013
                                                | FileFormatVersions.Office2016
                                                | FileFormatVersions.Office2019
                                                | FileFormatVersions.Office2021
                                                | FileFormatVersions.Microsoft365,
                FileFormatVersions.Office2016 => FileFormatVersions.Office2016
                                                | FileFormatVersions.Office2019
                                                | FileFormatVersions.Office2021
                                                | FileFormatVersions.Microsoft365,
                FileFormatVersions.Office2019 => FileFormatVersions.Office2019
                                                | FileFormatVersions.Office2021
                                                | FileFormatVersions.Microsoft365,
                FileFormatVersions.Office2021 => FileFormatVersions.Office2021
                                                | FileFormatVersions.Microsoft365,
                FileFormatVersions.Microsoft365 => FileFormatVersions.Microsoft365,
                _ => throw new ArgumentOutOfRangeException(nameof(version)),
            };

        /// <summary>
        /// Throws if the <see cref="OpenXmlPart"/> is not supported in the given version
        /// </summary>
        /// <param name="version">Version to check</param>
        /// <param name="part">Part to validate</param>
        public static void ThrowIfNotInVersion(this FileFormatVersions version, OpenXmlPart part)
        {
            version.ThrowExceptionIfFileFormatNotSupported(nameof(version));

            if (!part.IsInVersion(version))
            {
                throw new InvalidOperationException(SR.Format(ExceptionMessages.PartIsNotInOfficeVersion, version.GetOfficeYear()));
            }
        }

        /// <summary>
        /// Throws if the <see cref="OpenXmlElement"/> is not supported in the given version
        /// </summary>
        /// <param name="version">Version to check</param>
        /// <param name="element">Element to validate</param>
        public static void ThrowIfNotInVersion(this FileFormatVersions version, OpenXmlElement element)
        {
            version.ThrowExceptionIfFileFormatNotSupported(nameof(version));

            if (!element.IsInVersion(version))
            {
                throw new InvalidOperationException(SR.Format(ExceptionMessages.ElementIsNotInOfficeVersion, version.GetOfficeYear()));
            }
        }

        /// <summary>
        /// Check if a given version is at least a specified version
        /// </summary>
        /// <param name="version">Version to check</param>
        /// <param name="minimum">Minimum version expected</param>
        /// <returns>True if supplied version is at least of the specified version, otherwise false</returns>
        public static bool AtLeast(this FileFormatVersions version, FileFormatVersions minimum)
        {
            static int MapToInteger(FileFormatVersions v, string name)
            {
                if ((FileFormatVersions.Office2007 & v) == FileFormatVersions.Office2007)
                {
                    return 1;
                }

                if ((FileFormatVersions.Office2010 & v) == FileFormatVersions.Office2010)
                {
                    return 2;
                }

                if ((FileFormatVersions.Office2013 & v) == FileFormatVersions.Office2013)
                {
                    return 3;
                }

                if ((FileFormatVersions.Office2016 & v) == FileFormatVersions.Office2016)
                {
                    return 4;
                }

                if ((FileFormatVersions.Office2019 & v) == FileFormatVersions.Office2019)
                {
                    return 5;
                }

                if ((FileFormatVersions.Office2021 & v) == FileFormatVersions.Office2021)
                {
                    return 6;
                }

                if ((FileFormatVersions.Microsoft365 & v) == FileFormatVersions.Microsoft365)
                {
                    return 7;
                }

                throw new ArgumentOutOfRangeException(name);
            }

            if (version == FileFormatVersions.None || minimum == FileFormatVersions.None)
            {
                return false;
            }

            return MapToInteger(version, nameof(version)) >= MapToInteger(minimum, nameof(minimum));
        }

        /// <summary>
        /// Determines whether the source FileFormatVersions includes the target FileFormatVersions.
        /// </summary>
        /// <param name="source">The source FileFormatVersions to be tested.</param>
        /// <param name="target">The target FileFormatVersions be tested against.</param>
        /// <returns>Returns true when (source &amp; target) == target.</returns>
        internal static bool Includes(this FileFormatVersions source, FileFormatVersions target)
        {
            return (source & target) == target;
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if the specified FileFormatVersions is not supported.
        /// </summary>
        /// <param name="fileFormat">The specified FileFormatVersions.</param>
        /// <param name="parameterName">The name of the parameter for ArgumentOutOfRangeException.</param>
        internal static void ThrowExceptionIfFileFormatNotSupported(this FileFormatVersions fileFormat, string parameterName)
        {
            if (!fileFormat.Any())
            {
                throw new ArgumentOutOfRangeException(parameterName, SR.Format(ExceptionMessages.FileFormatNotSupported, fileFormat));
            }
        }

        private static string GetOfficeYear(this FileFormatVersions version)
        {
            return version.ToString().Substring("Office".Length);
        }
    }
}
