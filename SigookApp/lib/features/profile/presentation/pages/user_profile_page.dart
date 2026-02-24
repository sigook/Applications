import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/providers/file_picker_provider.dart';
import '../../../../core/services/file_picker_service.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/widgets/navbar_logo.dart';
import '../../../../core/widgets/profile_section_card.dart';
import '../../../../core/widgets/profile_info_row.dart';
import '../../../../core/widgets/loading_indicator.dart';
import '../../../../core/widgets/error_state_widget.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../domain/entities/worker_profile.dart';
import '../../domain/usecases/update_worker_profile.dart';
import '../providers/cached_worker_profile_provider.dart';
import '../providers/profile_providers.dart';
import '../widgets/profile_header.dart';

class UserProfilePage extends ConsumerStatefulWidget {
  const UserProfilePage({super.key});

  @override
  ConsumerState<UserProfilePage> createState() => _UserProfilePageState();
}

class _UserProfilePageState extends ConsumerState<UserProfilePage> {
  bool _isEditing = false;
  bool _isSaving = false;

  // Document deletion flags
  bool _deleteSinFile = false;
  bool _deleteId1File = false;
  bool _deleteId2File = false;
  bool _deletePoliceCheck = false;

  // Pending document replacements (locally picked files, not yet uploaded)
  PickedFileData? _replaceSinFile;
  PickedFileData? _replaceId1File;
  PickedFileData? _replaceId2File;
  PickedFileData? _replacePoliceCheckFile;

  // Text controllers for editable fields
  final _firstNameController = TextEditingController();
  final _middleNameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _secondLastNameController = TextEditingController();
  final _mobileNumberController = TextEditingController();
  final _phoneController = TextEditingController();
  final _emailController = TextEditingController();
  final _addressController = TextEditingController();
  final _postalCodeController = TextEditingController();
  final _emergencyNameController = TextEditingController();
  final _emergencyLastNameController = TextEditingController();
  final _emergencyPhoneController = TextEditingController();
  final _socialInsuranceController = TextEditingController();
  final _idNumber1Controller = TextEditingController();
  final _idNumber2Controller = TextEditingController();

  @override
  void dispose() {
    _firstNameController.dispose();
    _middleNameController.dispose();
    _lastNameController.dispose();
    _secondLastNameController.dispose();
    _mobileNumberController.dispose();
    _phoneController.dispose();
    _emailController.dispose();
    _addressController.dispose();
    _postalCodeController.dispose();
    _emergencyNameController.dispose();
    _emergencyLastNameController.dispose();
    _emergencyPhoneController.dispose();
    _socialInsuranceController.dispose();
    _idNumber1Controller.dispose();
    _idNumber2Controller.dispose();
    super.dispose();
  }

  void _populateControllers(WorkerProfile? profile) {
    _firstNameController.text = profile?.firstName ?? '';
    _middleNameController.text = profile?.middleName ?? '';
    _lastNameController.text = profile?.lastName ?? '';
    _secondLastNameController.text = profile?.secondLastName ?? '';
    _mobileNumberController.text = profile?.mobileNumber ?? '';
    _phoneController.text = profile?.phone ?? '';
    _emailController.text = profile?.email ?? '';
    _addressController.text = profile?.address ?? '';
    _postalCodeController.text = profile?.postalCode ?? '';
    _emergencyNameController.text = profile?.contactEmergencyName ?? '';
    _emergencyLastNameController.text = profile?.contactEmergencyLastName ?? '';
    _emergencyPhoneController.text = profile?.contactEmergencyPhone ?? '';
    _socialInsuranceController.text = profile?.socialInsurance ?? '';
    _idNumber1Controller.text = profile?.identificationNumber1 ?? '';
    _idNumber2Controller.text = profile?.identificationNumber2 ?? '';
  }

  void _enterEditMode(WorkerProfile? profile) {
    _populateControllers(profile);
    setState(() {
      _isEditing = true;
      _deleteSinFile = false;
      _deleteId1File = false;
      _deleteId2File = false;
      _deletePoliceCheck = false;
      _replaceSinFile = null;
      _replaceId1File = null;
      _replaceId2File = null;
      _replacePoliceCheckFile = null;
    });
  }

  void _cancelEditMode() {
    setState(() {
      _isEditing = false;
      _deleteSinFile = false;
      _deleteId1File = false;
      _deleteId2File = false;
      _deletePoliceCheck = false;
      _replaceSinFile = null;
      _replaceId1File = null;
      _replaceId2File = null;
      _replacePoliceCheckFile = null;
    });
  }

  Future<void> _pickFileFor(String docType) async {
    final result = await ref.read(filePickerServiceProvider).pickFile(
      allowedExtensions: ['pdf', 'jpg', 'jpeg', 'png'],
    );
    if (!result.isSuccess || result.file == null) return;
    setState(() {
      switch (docType) {
        case 'sinFile':
          _replaceSinFile = result.file;
          _deleteSinFile = false;
        case 'id1File':
          _replaceId1File = result.file;
          _deleteId1File = false;
        case 'id2File':
          _replaceId2File = result.file;
          _deleteId2File = false;
        case 'policeCheckFile':
          _replacePoliceCheckFile = result.file;
          _deletePoliceCheck = false;
      }
    });
  }

  Future<void> _saveChanges() async {
    setState(() {
      _isSaving = true;
    });

    final updateUseCase = ref.read(updateWorkerProfileUseCaseProvider);

    final newFilePaths = <String, String>{
      if (_replaceSinFile != null) 'sinFile': _replaceSinFile!.path,
      if (_replaceId1File != null) 'id1File': _replaceId1File!.path,
      if (_replaceId2File != null) 'id2File': _replaceId2File!.path,
      if (_replacePoliceCheckFile != null)
        'policeCheckFile': _replacePoliceCheckFile!.path,
    };

    final result = await updateUseCase(
      UpdateWorkerProfileParams(
        editedFields: {
          'firstName': _firstNameController.text,
          'middleName': _middleNameController.text,
          'lastName': _lastNameController.text,
          'secondLastName': _secondLastNameController.text,
          'mobileNumber': _mobileNumberController.text,
          'phone': _phoneController.text,
          'email': _emailController.text,
          'address': _addressController.text,
          'postalCode': _postalCodeController.text,
          'contactEmergencyName': _emergencyNameController.text,
          'contactEmergencyLastName': _emergencyLastNameController.text,
          'contactEmergencyPhone': _emergencyPhoneController.text,
          'socialInsurance': _socialInsuranceController.text,
          'identificationNumber1': _idNumber1Controller.text,
          'identificationNumber2': _idNumber2Controller.text,
          if (_deleteSinFile) '_deleteSinFile': 'true',
          if (_deleteId1File) '_deleteId1File': 'true',
          if (_deleteId2File) '_deleteId2File': 'true',
          if (_deletePoliceCheck) '_deletePoliceCheck': 'true',
        },
        newFilePaths: newFilePaths.isNotEmpty ? newFilePaths : null,
      ),
    );

    if (!mounted) return;

    setState(() {
      _isSaving = false;
    });

    result.fold(
      (failure) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to save: ${failure.message}'),
            backgroundColor: AppTheme.errorRed,
            behavior: SnackBarBehavior.floating,
          ),
        );
      },
      (_) {
        setState(() {
          _isEditing = false;
          _replaceSinFile = null;
          _replaceId1File = null;
          _replaceId2File = null;
          _replacePoliceCheckFile = null;
        });
        // Refresh profile data from server
        ref.invalidate(cachedWorkerProfileProvider);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Profile updated successfully!'),
            backgroundColor: AppTheme.successGreen,
            behavior: SnackBarBehavior.floating,
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final profileAsync = ref.watch(cachedWorkerProfileProvider);

    return Scaffold(
      backgroundColor: AppTheme.surfaceGrey,
      appBar: AppBar(
        backgroundColor: AppTheme.primaryBlue,
        foregroundColor: Colors.white,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => context.go(AppRoutes.jobs),
        ),
        title: const NavbarLogo(),
        actions: [
          profileAsync.whenOrNull(
                data: (profile) => IconButton(
                  icon: Icon(_isEditing ? Icons.close : Icons.edit),
                  onPressed: () {
                    if (_isEditing) {
                      _cancelEditMode();
                    } else {
                      _enterEditMode(profile);
                    }
                  },
                ),
              ) ??
              const SizedBox.shrink(),
        ],
      ),
      body: profileAsync.when(
        data: (profile) => SingleChildScrollView(
          child: Column(
            children: [
              ProfileHeader(
                name: profile?.fullName ?? 'User',
                email: profile?.email ?? '',
                photoUrl: profile?.profilePhotoUrl,
                isEditing: _isEditing,
              ),
              const SizedBox(height: 16),
              _buildPersonalInfoSection(profile),
              const SizedBox(height: 12),
              _buildContactSection(profile),
              const SizedBox(height: 12),
              _buildLocationSection(profile),
              const SizedBox(height: 12),
              _buildPreferencesSection(profile),
              const SizedBox(height: 12),
              _buildDocumentsSection(profile),
              const SizedBox(height: 12),
              _buildCommentsSection(),
              const SizedBox(height: 24),
              _buildActionButtons(),
              const SizedBox(height: 32),
            ],
          ),
        ),
        loading: () => const LoadingIndicator(message: 'Loading profile...'),
        error: (_, __) => ErrorStateWidget(
          title: 'Failed to load profile',
          message: 'Unable to retrieve your profile information',
          onRetry: () => ref.refresh(cachedWorkerProfileProvider),
        ),
      ),
    );
  }

  Widget _buildPersonalInfoSection(WorkerProfile? profile) {
    return ProfileSectionCard(
      title: 'Personal Information',
      icon: Icons.person_outline,
      iconGradient: const [AppTheme.primaryBlue, AppTheme.tertiaryBlue],
      children: [
        ProfileInfoRow(
          label: 'First Name',
          value: profile?.firstName ?? 'N/A',
          icon: Icons.badge_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _firstNameController : null,
        ),
        ProfileInfoRow(
          label: 'Middle Name',
          value: profile?.middleName ?? '',
          icon: Icons.badge_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _middleNameController : null,
        ),
        ProfileInfoRow(
          label: 'Last Name',
          value: profile?.lastName ?? 'N/A',
          icon: Icons.badge_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _lastNameController : null,
        ),
        ProfileInfoRow(
          label: 'Second Last Name',
          value: profile?.secondLastName ?? '',
          icon: Icons.badge_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _secondLastNameController : null,
        ),
        ProfileInfoRow(
          label: 'Date of Birth',
          value: profile?.formattedBirthDay ?? 'N/A',
          icon: Icons.cake_outlined,
        ),
        ProfileInfoRow(
          label: 'Gender',
          value: profile?.gender ?? 'N/A',
          icon: Icons.wc_outlined,
        ),
        ProfileInfoRow(
          label: 'Approved to Work',
          value: profile?.approvedToWork == true ? 'Yes' : 'No',
          icon: Icons.verified_outlined,
        ),
        if (profile?.punchCardId != null && profile!.punchCardId!.isNotEmpty)
          ProfileInfoRow(
            label: 'Punch Card ID',
            value: profile.punchCardId!,
            icon: Icons.credit_card_outlined,
          ),
      ],
    );
  }

  Widget _buildContactSection(WorkerProfile? profile) {
    return ProfileSectionCard(
      title: 'Contact Information',
      icon: Icons.contact_phone_outlined,
      iconGradient: const [Color(0xFF4CAF50), Color(0xFF81C784)],
      children: [
        ProfileInfoRow(
          label: 'Mobile Number',
          value: profile?.mobileNumber ?? 'N/A',
          icon: Icons.phone_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _mobileNumberController : null,
        ),
        ProfileInfoRow(
          label: 'Phone',
          value: profile?.phone ?? '',
          icon: Icons.phone_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _phoneController : null,
        ),
        ProfileInfoRow(
          label: 'Email',
          value: profile?.email ?? 'N/A',
          icon: Icons.email_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _emailController : null,
        ),
        ProfileInfoRow(
          label: 'Emergency Contact Name',
          value: profile?.contactEmergencyName ?? '',
          icon: Icons.emergency_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _emergencyNameController : null,
        ),
        ProfileInfoRow(
          label: 'Emergency Contact Last Name',
          value: profile?.contactEmergencyLastName ?? '',
          icon: Icons.emergency_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _emergencyLastNameController : null,
        ),
        ProfileInfoRow(
          label: 'Emergency Phone',
          value: profile?.contactEmergencyPhone ?? '',
          icon: Icons.phone_callback_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _emergencyPhoneController : null,
        ),
      ],
    );
  }

  Widget _buildLocationSection(WorkerProfile? profile) {
    return ProfileSectionCard(
      title: 'Location Details',
      icon: Icons.location_on_outlined,
      iconGradient: const [Color(0xFFFF9800), Color(0xFFFFB74D)],
      children: [
        ProfileInfoRow(
          label: 'Country',
          value: profile?.country ?? 'N/A',
          icon: Icons.flag_outlined,
        ),
        ProfileInfoRow(
          label: 'State/Province',
          value: profile?.province ?? 'N/A',
          icon: Icons.map_outlined,
        ),
        ProfileInfoRow(
          label: 'City',
          value: profile?.city ?? 'N/A',
          icon: Icons.location_city_outlined,
        ),
        ProfileInfoRow(
          label: 'Address',
          value: profile?.address ?? 'N/A',
          icon: Icons.home_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _addressController : null,
        ),
        ProfileInfoRow(
          label: 'Postal Code',
          value: profile?.postalCode ?? 'N/A',
          icon: Icons.markunread_mailbox_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _postalCodeController : null,
        ),
      ],
    );
  }

  Widget _buildPreferencesSection(WorkerProfile? profile) {
    return ProfileSectionCard(
      title: 'Work Preferences',
      icon: Icons.work_outline,
      iconGradient: const [Color(0xFF9C27B0), Color(0xFFBA68C8)],
      children: [
        ProfileInfoRow(
          label: 'Availability',
          value: profile != null && profile.availabilities.isNotEmpty
              ? profile.availabilities.join(', ')
              : 'N/A',
          icon: Icons.schedule_outlined,
        ),
        ProfileInfoRow(
          label: 'Available Days',
          value: profile != null && profile.availabilityDays.isNotEmpty
              ? profile.availabilityDays.join(', ')
              : 'N/A',
          icon: Icons.calendar_today_outlined,
        ),
        ProfileInfoRow(
          label: 'Preferred Time',
          value: profile != null && profile.availabilityTimes.isNotEmpty
              ? profile.availabilityTimes.join(', ')
              : 'N/A',
          icon: Icons.access_time_outlined,
        ),
        ProfileInfoRow(
          label: 'Lifting Capacity',
          value: profile?.liftCapacity ?? 'N/A',
          icon: Icons.fitness_center_outlined,
        ),
        ProfileInfoRow(
          label: 'Has Vehicle',
          value: profile?.hasVehicle == true ? 'Yes' : 'No',
          icon: Icons.directions_car_outlined,
        ),
        ProfileInfoRow(
          label: 'Languages',
          value: profile != null && profile.languages.isNotEmpty
              ? profile.languages.join(', ')
              : 'N/A',
          icon: Icons.language_outlined,
        ),
        ProfileInfoRow(
          label: 'Skills',
          value: profile != null && profile.skills.isNotEmpty
              ? profile.skills.join(', ')
              : 'N/A',
          icon: Icons.stars_outlined,
        ),
      ],
    );
  }

  Widget _buildDocumentFileRow({
    required String label,
    required String? fileName,
    required bool isMarkedForDeletion,
    required PickedFileData? pendingFile,
    required VoidCallback onDelete,
    required VoidCallback onUndo,
    required String docType,
    required VoidCallback onClearPick,
  }) {
    final hasFile = fileName != null && fileName.isNotEmpty;
    final hasPending = pendingFile != null;

    if (!_isEditing) {
      return ProfileInfoRow(
        label: label,
        value: hasFile ? fileName : 'Not uploaded',
        icon: Icons.attach_file_outlined,
      );
    }

    // Determine display name and style
    final String displayName;
    final TextStyle nameStyle;
    final String? statusText;

    if (hasPending) {
      displayName = pendingFile.name;
      nameStyle = const TextStyle(fontSize: 14, color: AppTheme.primaryBlue);
      statusText = hasFile ? 'Will replace on save' : 'Will upload on save';
    } else if (isMarkedForDeletion) {
      displayName = fileName!;
      nameStyle = const TextStyle(
        fontSize: 14,
        color: AppTheme.errorRed,
        decoration: TextDecoration.lineThrough,
      );
      statusText = 'Will be removed on save';
    } else {
      displayName = hasFile ? fileName : 'Not uploaded';
      nameStyle = TextStyle(
        fontSize: 14,
        color: hasFile ? AppTheme.textDark : Colors.grey,
      );
      statusText = null;
    }

    // Determine action buttons
    Widget actions;
    if (hasPending || isMarkedForDeletion) {
      actions = TextButton.icon(
        onPressed: hasPending ? onClearPick : onUndo,
        icon: const Icon(Icons.undo, size: 16),
        label: const Text('Undo'),
        style: TextButton.styleFrom(
          foregroundColor: AppTheme.primaryBlue,
          padding: const EdgeInsets.symmetric(horizontal: 8),
        ),
      );
    } else if (hasFile) {
      actions = Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          IconButton(
            onPressed: onDelete,
            icon: const Icon(Icons.delete_outline, size: 20),
            color: AppTheme.errorRed,
            tooltip: 'Remove',
            padding: EdgeInsets.zero,
            constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
          ),
          IconButton(
            onPressed: () => _pickFileFor(docType),
            icon: const Icon(Icons.swap_horiz, size: 20),
            color: AppTheme.primaryBlue,
            tooltip: 'Replace',
            padding: EdgeInsets.zero,
            constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
          ),
        ],
      );
    } else {
      actions = IconButton(
        onPressed: () => _pickFileFor(docType),
        icon: const Icon(Icons.upload_file, size: 20),
        color: AppTheme.primaryBlue,
        tooltip: 'Upload',
        padding: EdgeInsets.zero,
        constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
      );
    }

    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6, horizontal: 4),
      child: Row(
        children: [
          const Icon(Icons.attach_file_outlined, size: 18, color: Colors.grey),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: const TextStyle(
                    fontSize: 12,
                    color: Colors.grey,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(height: 2),
                Text(displayName, style: nameStyle),
                if (statusText != null)
                  Text(
                    statusText,
                    style: TextStyle(
                      fontSize: 11,
                      color: hasPending
                          ? AppTheme.primaryBlue
                          : AppTheme.errorRed,
                    ),
                  ),
              ],
            ),
          ),
          actions,
        ],
      ),
    );
  }

  Widget _buildDocumentsSection(WorkerProfile? profile) {
    return ProfileSectionCard(
      title: 'Documents & Account',
      icon: Icons.description_outlined,
      iconGradient: const [Color(0xFFF44336), Color(0xFFE57373)],
      children: [
        ProfileInfoRow(
          label: 'Social Insurance (SIN)',
          value: profile?.maskedSocialInsurance ?? 'N/A',
          icon: Icons.security_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _socialInsuranceController : null,
        ),
        ProfileInfoRow(
          label: 'SIN Expires',
          value: profile?.socialInsuranceExpire == true ? 'Yes' : 'No',
          icon: Icons.event_busy_outlined,
        ),
        if (profile?.socialInsuranceExpire == true)
          ProfileInfoRow(
            label: 'SIN Due Date',
            value: profile?.formattedDueDate ?? 'N/A',
            icon: Icons.calendar_today_outlined,
          ),
        _buildDocumentFileRow(
          label: 'SIN Document',
          fileName: profile?.socialInsuranceFileName,
          isMarkedForDeletion: _deleteSinFile,
          pendingFile: _replaceSinFile,
          docType: 'sinFile',
          onDelete: () => setState(() {
            _deleteSinFile = true;
            _replaceSinFile = null;
          }),
          onUndo: () => setState(() => _deleteSinFile = false),
          onClearPick: () => setState(() => _replaceSinFile = null),
        ),
        if (profile?.identificationType1 != null)
          ProfileInfoRow(
            label: '${profile!.identificationType1!} (ID 1)',
            value: profile.maskedIdNumber1,
            icon: Icons.credit_card_outlined,
            isEditing: _isEditing,
            controller: _isEditing ? _idNumber1Controller : null,
          ),
        _buildDocumentFileRow(
          label: 'ID 1 Document',
          fileName: profile?.identificationType1FileName,
          isMarkedForDeletion: _deleteId1File,
          pendingFile: _replaceId1File,
          docType: 'id1File',
          onDelete: () => setState(() {
            _deleteId1File = true;
            _replaceId1File = null;
          }),
          onUndo: () => setState(() => _deleteId1File = false),
          onClearPick: () => setState(() => _replaceId1File = null),
        ),
        if (profile?.identificationType2 != null)
          ProfileInfoRow(
            label: '${profile!.identificationType2!} (ID 2)',
            value: profile.maskedIdNumber2,
            icon: Icons.credit_card_outlined,
            isEditing: _isEditing,
            controller: _isEditing ? _idNumber2Controller : null,
          ),
        _buildDocumentFileRow(
          label: 'ID 2 Document',
          fileName: profile?.identificationType2FileName,
          isMarkedForDeletion: _deleteId2File,
          pendingFile: _replaceId2File,
          docType: 'id2File',
          onDelete: () => setState(() {
            _deleteId2File = true;
            _replaceId2File = null;
          }),
          onUndo: () => setState(() => _deleteId2File = false),
          onClearPick: () => setState(() => _replaceId2File = null),
        ),
        _buildDocumentFileRow(
          label: 'Police Check Document',
          fileName: profile?.policeCheckBackgroundFileName,
          isMarkedForDeletion: _deletePoliceCheck,
          pendingFile: _replacePoliceCheckFile,
          docType: 'policeCheckFile',
          onDelete: () => setState(() {
            _deletePoliceCheck = true;
            _replacePoliceCheckFile = null;
          }),
          onUndo: () => setState(() => _deletePoliceCheck = false),
          onClearPick: () => setState(() => _replacePoliceCheckFile = null),
        ),
        ProfileInfoRow(
          label: 'Resume',
          value: profile?.hasResume == true ? 'On file' : 'Not provided',
          icon: Icons.description_outlined,
        ),
        ProfileInfoRow(
          label: 'Worker ID',
          value: profile?.numberId?.toString() ?? 'N/A',
          icon: Icons.numbers_outlined,
        ),
      ],
    );
  }

  Widget _buildCommentsSection() {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 10,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  gradient: const LinearGradient(
                    colors: [Color(0xFF2196F3), Color(0xFF64B5F6)],
                  ),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(
                  Icons.comment_outlined,
                  color: Colors.white,
                  size: 20,
                ),
              ),
              const SizedBox(width: 12),
              const Text(
                'Additional Comments',
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                  color: AppTheme.textDark,
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          if (_isEditing)
            TextField(
              maxLines: 4,
              decoration: InputDecoration(
                hintText: 'Add any additional information...',
                filled: true,
                fillColor: Colors.grey.shade50,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: BorderSide(color: Colors.grey.shade300),
                ),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: BorderSide(color: Colors.grey.shade300),
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: const BorderSide(
                    color: AppTheme.primaryBlue,
                    width: 2,
                  ),
                ),
              ),
            )
          else
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Colors.grey.shade50,
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: Colors.grey.shade200),
              ),
              child: Text(
                'No additional comments.',
                style: TextStyle(
                  fontSize: 14,
                  color: Colors.grey.shade700,
                  height: 1.5,
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildActionButtons() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Column(
        children: [
          if (_isEditing)
            SizedBox(
              width: double.infinity,
              height: 56,
              child: ElevatedButton(
                onPressed: _isSaving ? null : _saveChanges,
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.primaryBlue,
                  foregroundColor: Colors.white,
                  elevation: 2,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(16),
                  ),
                ),
                child: _isSaving
                    ? const SizedBox(
                        width: 24,
                        height: 24,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          color: Colors.white,
                        ),
                      )
                    : const Text(
                        'Save Changes',
                        style: TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          letterSpacing: 0.5,
                        ),
                      ),
              ),
            ),
          if (_isEditing) const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            height: 56,
            child: OutlinedButton(
              onPressed: () => _showLogoutDialog(),
              style: OutlinedButton.styleFrom(
                foregroundColor: AppTheme.secondaryRed,
                side: const BorderSide(color: AppTheme.secondaryRed, width: 2),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
              ),
              child: const Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.logout, size: 22),
                  SizedBox(width: 12),
                  Text(
                    'Logout',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      letterSpacing: 0.5,
                    ),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            height: 56,
            child: OutlinedButton(
              onPressed: null,
              style: OutlinedButton.styleFrom(
                foregroundColor: AppTheme.errorRed,
                side: const BorderSide(color: AppTheme.errorRed, width: 2),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
              ),
              child: const Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.delete_forever, size: 22),
                  SizedBox(width: 12),
                  Text(
                    'Delete Account',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      letterSpacing: 0.5,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _showLogoutDialog() async {
    final shouldLogout = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Logout'),
        content: const Text('Are you sure you want to logout?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.of(context).pop(true),
            style: ElevatedButton.styleFrom(
              backgroundColor: AppTheme.secondaryRed,
            ),
            child: const Text('Logout'),
          ),
        ],
      ),
    );

    if (shouldLogout == true && mounted) {
      showDialog(
        context: context,
        barrierDismissible: false,
        builder: (context) => const Center(
          child: CircularProgressIndicator(color: AppTheme.primaryBlue),
        ),
      );

      await ref.read(authViewModelProvider.notifier).logout();

      if (mounted) {
        Navigator.of(context).pop();
        context.go(AppRoutes.welcome);
      }
    }
  }

}
