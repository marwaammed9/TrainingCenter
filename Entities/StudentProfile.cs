using System;
using System.Collections.Generic;

namespace TrainingCenter.Entities;

// ==========================================================
// StudentProfile Entity Class
// ----------------------------------------------------------
// This class represents the StudentProfiles table
// in the database.
//
// Each object created from this class represents
// extra profile information for one student.
//
// Why separate table?
// Because not all student data belongs in Students table.
// Optional details can be stored here.
//
// Example:
// Address, Bio, LinkedIn profile...etc
// ==========================================================
public partial class StudentProfile
{
    // ------------------------------------------------------
    // Primary Key + Foreign Key
    // ------------------------------------------------------
    // StudentId is special here:
    //
    // 🔹 Primary Key of StudentProfiles table
    // 🔹 Foreign Key linked to Students table
    //
    // This creates a One-to-One relationship.
    //
    // Meaning:
    // One student can have only one profile.
    // ------------------------------------------------------
    public int StudentId { get; set; }

    // ------------------------------------------------------
    // Address
    // ------------------------------------------------------
    // Optional home / mailing address.
    //
    // Nullable (?) means it can be empty.
    //
    // Example:
    // Amman - Jordan
    // ------------------------------------------------------
    public string? Address { get; set; }

    // ------------------------------------------------------
    // City
    // ------------------------------------------------------
    // Optional city name.
    //
    // Examples:
    // Zarqa
    // Amman
    // Dubai
    // ------------------------------------------------------
    public string? City { get; set; }

    // ------------------------------------------------------
    // Country
    // ------------------------------------------------------
    // Optional country name.
    //
    // Examples:
    // Jordan
    // Saudi Arabia
    // UAE
    // ------------------------------------------------------
    public string? Country { get; set; }

    // ------------------------------------------------------
    // Bio
    // ------------------------------------------------------
    // Optional short personal summary.
    //
    // Example:
    // "Interested in backend development
    // and database systems."
    // ------------------------------------------------------
    public string? Bio { get; set; }

    // ------------------------------------------------------
    // LinkedInUrl
    // ------------------------------------------------------
    // Optional LinkedIn profile link.
    //
    // Example:
    // https://linkedin.com/in/student-name
    // ------------------------------------------------------
    public string? LinkedInUrl { get; set; }

    // ======================================================
    // Navigation Property
    // ------------------------------------------------------
    // Represents the related student record.
    // ======================================================

    // ------------------------------------------------------
    // Student Relationship
    // ------------------------------------------------------
    // Each profile belongs to one student.
    //
    // Example:
    // profile.Student.FirstName
    // ------------------------------------------------------
    public virtual Student Student { get; set; } = null!;
}
