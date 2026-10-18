enum ProfileSection { photo, sin, documents }

enum WorkerRequirement {
  profileImage(
    'ProfileImage',
    'Profile photo',
    'Tap your photo at the top of the profile',
    ProfileSection.photo,
  ),
  taxCategory(
    'TaxCategory',
    'Tax category',
    'Set by your agency, no action needed from you',
    null,
  ),
  socialInsuranceNumber(
    'SocialInsuranceNumber',
    'Social Insurance number',
    'Personal › Social Insurance',
    ProfileSection.sin,
  ),
  socialInsuranceFile(
    'SocialInsuranceFile',
    'Social Insurance document',
    'Personal › Social Insurance',
    ProfileSection.sin,
  ),
  socialInsuranceExpired(
    'SocialInsuranceExpired',
    'Social Insurance expired',
    'Personal › Social Insurance',
    ProfileSection.sin,
  ),
  identification(
    'Identification',
    'Identification document',
    'Personal › Documents',
    ProfileSection.documents,
  );

  const WorkerRequirement(this.code, this.label, this.hint, this.section);

  final String code;
  final String label;
  final String hint;
  final ProfileSection? section;

  static WorkerRequirement? fromCode(String code) {
    for (final value in values) {
      if (value.code == code) return value;
    }
    return null;
  }
}
