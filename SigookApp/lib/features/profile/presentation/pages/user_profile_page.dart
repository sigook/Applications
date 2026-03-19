import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:mask_text_input_formatter/mask_text_input_formatter.dart';
import 'package:package_info_plus/package_info_plus.dart';
import 'package:webview_flutter/webview_flutter.dart';
import '../../../../core/providers/file_picker_provider.dart';
import '../../../../core/services/file_picker_service.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/widgets/navbar_logo.dart';
import '../../../../core/widgets/profile_section_card.dart';
import '../../../../core/widgets/profile_info_row.dart';
import '../../../../core/widgets/loading_indicator.dart';
import '../../../../core/widgets/error_state_widget.dart';
import '../../../auth/presentation/pages/logout_webview_page.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../../../registration/presentation/widgets/location_selector.dart';
import '../../../registration/domain/entities/country.dart';
import '../../../registration/domain/entities/province.dart';
import '../../../registration/domain/entities/city.dart';
import '../../domain/entities/worker_profile.dart';
import '../../domain/usecases/update_worker_profile.dart';
import '../providers/cached_worker_profile_provider.dart';
import '../providers/profile_providers.dart';
import '../widgets/profile_header.dart';
import '../widgets/searchable_toggle_list.dart';

class UserProfilePage extends ConsumerStatefulWidget {
  const UserProfilePage({super.key});

  @override
  ConsumerState<UserProfilePage> createState() => _UserProfilePageState();
}

class _UserProfilePageState extends ConsumerState<UserProfilePage>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController;
  ProfileSection? _editingSection;
  bool _isSaving = false;
  String _appVersion = '';

  // Document deletion flags
  bool _deleteSinFile = false;
  bool _deleteId1File = false;
  bool _deleteId2File = false;

  // Pending document replacements
  PickedFileData? _replaceSinFile;
  PickedFileData? _replaceId1File;
  PickedFileData? _replaceId2File;

  // Location editing state
  Country? _editCountry;
  Province? _editProvince;
  City? _editCity;

  // Preferences editing state
  Set<String> _editAvailabilityIds = {};
  Set<String> _editAvailabilityTimeIds = {};
  Set<String> _editAvailabilityDayIds = {};
  String? _editLiftId;
  bool _editHasVehicle = false;
  Set<String> _editLanguageIds = {};
  Set<String> _editSkillIds = {};

  // Phone mask formatters
  final _mobileMaskFormatter = MaskTextInputFormatter(
    mask: '### ### ####',
    filter: {'#': RegExp(r'[0-9]')},
    type: MaskAutoCompletionType.lazy,
  );
  final _phoneMaskFormatter = MaskTextInputFormatter(
    mask: '### ### ####',
    filter: {'#': RegExp(r'[0-9]')},
    type: MaskAutoCompletionType.lazy,
  );
  final _emergencyPhoneMaskFormatter = MaskTextInputFormatter(
    mask: '### ### ####',
    filter: {'#': RegExp(r'[0-9]')},
    type: MaskAutoCompletionType.lazy,
  );

  // Text controllers
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
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _loadAppVersion();
  }

  Future<void> _loadAppVersion() async {
    final info = await PackageInfo.fromPlatform();
    if (mounted) {
      setState(() {
        _appVersion = 'v${info.version} (${info.buildNumber})';
      });
    }
  }

  @override
  void dispose() {
    _tabController.dispose();
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
    _mobileNumberController.text =
        _mobileMaskFormatter.maskText(profile?.mobileNumber ?? '');
    _phoneController.text =
        _phoneMaskFormatter.maskText(profile?.phone ?? '');
    _emailController.text = profile?.email ?? '';
    _addressController.text = profile?.address ?? '';
    _postalCodeController.text = profile?.postalCode ?? '';
    _emergencyNameController.text = profile?.contactEmergencyName ?? '';
    _emergencyLastNameController.text = profile?.contactEmergencyLastName ?? '';
    _emergencyPhoneController.text =
        _emergencyPhoneMaskFormatter.maskText(profile?.contactEmergencyPhone ?? '');
    _socialInsuranceController.text = profile?.socialInsurance ?? '';
    _idNumber1Controller.text = profile?.identificationNumber1 ?? '';
    _idNumber2Controller.text = profile?.identificationNumber2 ?? '';
  }

  void _startEditing(ProfileSection section, WorkerProfile? profile) {
    _populateControllers(profile);
    setState(() {
      _editingSection = section;
      _deleteSinFile = false;
      _deleteId1File = false;
      _deleteId2File = false;
      _replaceSinFile = null;
      _replaceId1File = null;
      _replaceId2File = null;
      _editCountry = null;
      _editProvince = null;
      _editCity = null;
      if (section == ProfileSection.personal && profile != null) {
        _editHasVehicle = profile.hasVehicle;
      }
      if (section == ProfileSection.preferences && profile != null) {
        _editAvailabilityIds = Set.from(profile.availabilityIds);
        _editAvailabilityTimeIds = Set.from(profile.availabilityTimeIds);
        _editAvailabilityDayIds = Set.from(profile.availabilityDayIds);
        _editLiftId = profile.liftId;
        _editLanguageIds = Set.from(profile.languageIds);
        _editSkillIds = Set.from(profile.skills);
      }
    });
  }

  void _cancelEditing() {
    setState(() {
      _editingSection = null;
      _deleteSinFile = false;
      _deleteId1File = false;
      _deleteId2File = false;
      _replaceSinFile = null;
      _replaceId1File = null;
      _replaceId2File = null;
      _editCountry = null;
      _editProvince = null;
      _editCity = null;
    });
  }

  Future<void> _saveSection(ProfileSection section) async {
    setState(() => _isSaving = true);

    final updateUseCase = ref.read(updateWorkerProfileUseCaseProvider);
    late final Map<String, String> editedFields;
    Map<String, String>? newFilePaths;

    switch (section) {
      case ProfileSection.personal:
        editedFields = {
          'firstName': _firstNameController.text,
          'middleName': _middleNameController.text,
          'lastName': _lastNameController.text,
          'secondLastName': _secondLastNameController.text,
          'hasVehicle': _editHasVehicle.toString(),
        };
      case ProfileSection.contact:
        editedFields = {
          'mobileNumber': _mobileMaskFormatter.getUnmaskedText(),
          'phone': _phoneMaskFormatter.getUnmaskedText(),
          'address': _addressController.text,
          'postalCode': _postalCodeController.text,
          if (_editCity?.id != null) 'cityId': _editCity!.id!,
        };
      case ProfileSection.sin:
        editedFields = {
          'socialInsurance': _socialInsuranceController.text,
          if (_deleteSinFile) '_deleteSinFile': 'true',
        };
        newFilePaths = {
          if (_replaceSinFile != null) 'sinFile': _replaceSinFile!.path,
        };
      case ProfileSection.documents:
        editedFields = {
          'identificationNumber1': _idNumber1Controller.text,
          'identificationNumber2': _idNumber2Controller.text,
          if (_deleteId1File) '_deleteId1File': 'true',
          if (_deleteId2File) '_deleteId2File': 'true',
        };
        newFilePaths = {
          if (_replaceId1File != null) 'id1File': _replaceId1File!.path,
          if (_replaceId2File != null) 'id2File': _replaceId2File!.path,
        };
      case ProfileSection.emergency:
        editedFields = {
          'contactEmergencyName': _emergencyNameController.text,
          'contactEmergencyLastName': _emergencyLastNameController.text,
          'contactEmergencyPhone': _emergencyPhoneMaskFormatter.getUnmaskedText(),
        };
      case ProfileSection.preferences:
        editedFields = {
          'availabilityIds': _editAvailabilityIds.join(','),
          'availabilityTimeIds': _editAvailabilityTimeIds.join(','),
          'availabilityDayIds': _editAvailabilityDayIds.join(','),
          if (_editLiftId != null) 'liftId': _editLiftId!,
          'languageIds': _editLanguageIds.join(','),
          'skillIds': _editSkillIds.join(','),
        };
    }

    final result = await updateUseCase(
      UpdateWorkerProfileParams(
        editedFields: editedFields,
        section: section,
        newFilePaths: newFilePaths?.isNotEmpty == true ? newFilePaths : null,
      ),
    );

    if (!mounted) return;
    setState(() => _isSaving = false);

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
        setState(() => _editingSection = null);
        ref.invalidate(cachedWorkerProfileProvider);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Changes saved successfully!'),
            backgroundColor: AppTheme.successGreen,
            behavior: SnackBarBehavior.floating,
          ),
        );
      },
    );
  }

  Widget _sectionEditActions(ProfileSection section, WorkerProfile? profile) {
    if (_editingSection == section) {
      return Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (_isSaving)
            const SizedBox(
              width: 20,
              height: 20,
              child: CircularProgressIndicator(
                strokeWidth: 2,
                color: AppTheme.primaryBlue,
              ),
            )
          else
            IconButton(
              icon: const Icon(Icons.check_circle_outline, color: AppTheme.primaryBlue),
              onPressed: () => _saveSection(section),
              padding: EdgeInsets.zero,
              constraints: const BoxConstraints(),
              tooltip: 'Save',
            ),
          const SizedBox(width: 8),
          IconButton(
            icon: Icon(Icons.cancel_outlined, color: Colors.grey.shade500),
            onPressed: _isSaving ? null : _cancelEditing,
            padding: EdgeInsets.zero,
            constraints: const BoxConstraints(),
            tooltip: 'Cancel',
          ),
        ],
      );
    }
    if (_editingSection != null) return const SizedBox.shrink();
    return IconButton(
      icon: const Icon(Icons.edit_outlined, color: AppTheme.primaryBlue, size: 20),
      onPressed: profile == null ? null : () => _startEditing(section, profile),
      padding: EdgeInsets.zero,
      constraints: const BoxConstraints(),
      tooltip: 'Edit',
    );
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
      }
    });
  }

  void _previewDocument(String url, String title) {
    final token = ref.read(authViewModelProvider).token?.accessToken;
    Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) => _DocumentPreviewPage(url: url, title: title, token: token),
      ),
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
          onPressed: () {
            notifyLogoFlash();
            context.go(AppRoutes.jobs);
          },
        ),
        title: const NavbarLogo(),
      ),
      body: profileAsync.when(
        data: (profile) => NestedScrollView(
          headerSliverBuilder: (context, innerBoxIsScrolled) => [
            SliverAppBar(
              // expandedHeight = profile content area (222) + TabBar (48)
              expandedHeight: 270.0,
              // Collapse to minimum — only the TabBar (bottom) stays pinned.
              // Using 1 instead of 0 avoids a 1px overflow rounding issue.
              toolbarHeight: 1.0,
              pinned: true,
              floating: false,
              automaticallyImplyLeading: false,
              backgroundColor: Colors.white,
              elevation: 2,
              shadowColor: Colors.black.withValues(alpha: 0.08),
              flexibleSpace: FlexibleSpaceBar(
                collapseMode: CollapseMode.pin,
                background: Builder(
                  builder: (context) {
                    final settings = context
                        .dependOnInheritedWidgetOfExactType<
                            FlexibleSpaceBarSettings>();
                    final t = settings != null
                        ? ((settings.currentExtent - settings.minExtent) /
                                (settings.maxExtent - settings.minExtent))
                            .clamp(0.0, 1.0)
                        : 1.0;
                    return ProfileHeader(
                      name: profile?.fullName ?? 'User',
                      email: profile?.email ?? '',
                      photoUrl: profile?.profilePhotoUrl,
                      collapseRatio: t,
                    );
                  },
                ),
              ),
              bottom: PreferredSize(
                preferredSize: const Size.fromHeight(48.0),
                child: Container(
                  color: Colors.white,
                  child: TabBar(
                    controller: _tabController,
                    labelColor: AppTheme.primaryBlue,
                    unselectedLabelColor: Colors.grey.shade600,
                    indicatorColor: AppTheme.primaryBlue,
                    indicatorWeight: 3,
                    labelStyle: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w600,
                    ),
                    unselectedLabelStyle: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w500,
                    ),
                    tabs: const [
                      Tab(text: 'Personal Details'),
                      Tab(text: 'Preferences'),
                    ],
                  ),
                ),
              ),
            ),
          ],
          body: TabBarView(
            controller: _tabController,
            children: [
              _buildPersonalDetailsTab(profile),
              _buildPreferencesTab(profile),
            ],
          ),
        ),
        loading: () => const LoadingIndicator(message: 'Loading profile...'),
        error: (_, _) => ErrorStateWidget(
          title: 'Failed to load profile',
          message: 'Unable to retrieve your profile information',
          onRetry: () => ref.refresh(cachedWorkerProfileProvider),
        ),
      ),
    );
  }

  // ── Tab builders ──────────────────────────────────────────────────────────

  Widget _buildPersonalDetailsTab(WorkerProfile? profile) {
    return ListView(
      padding: EdgeInsets.zero,
      children: [
        const SizedBox(height: 16),
        _buildBasicInfoSection(profile),
        const SizedBox(height: 12),
        _buildContactInfoSection(profile),
        const SizedBox(height: 12),
        _buildSinSection(profile),
        const SizedBox(height: 12),
        _buildDocumentsSection(profile),
        const SizedBox(height: 24),
      ],
    );
  }

  Widget _buildPreferencesTab(WorkerProfile? profile) {
    return ListView(
      padding: EdgeInsets.zero,
      children: [
        const SizedBox(height: 16),
        _buildPreferencesSection(profile),
        const SizedBox(height: 12),
        _buildEmergencyInfoSection(profile),
        const SizedBox(height: 12),
        _buildActionButtons(),
        const SizedBox(height: 16),
        if (_appVersion.isNotEmpty)
          Padding(
            padding: const EdgeInsets.only(bottom: 24),
            child: Center(
              child: Text(
                _appVersion,
                style: TextStyle(
                  fontSize: 12,
                  color: Colors.grey.shade500,
                ),
              ),
            ),
          ),
      ],
    );
  }

  // ── Personal Details sections ─────────────────────────────────────────────

  Widget _buildBasicInfoSection(WorkerProfile? profile) {
    final isEditing = _editingSection == ProfileSection.personal;
    return ProfileSectionCard(
      title: 'Basic Information',
      icon: Icons.person_outline,
      iconGradient: const [AppTheme.primaryBlue, AppTheme.tertiaryBlue],
      trailing: _sectionEditActions(ProfileSection.personal, profile),
      children: [
        if (!isEditing)
          ProfileInfoRow(
            label: 'Full Name',
            value: profile?.fullName.isNotEmpty == true ? profile!.fullName : 'N/A',
            icon: Icons.badge_outlined,
          )
        else ...[
          ProfileInfoRow(
            label: 'First Name',
            value: profile?.firstName ?? 'N/A',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _firstNameController,
          ),
          ProfileInfoRow(
            label: 'Middle Name',
            value: profile?.middleName ?? '',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _middleNameController,
          ),
          ProfileInfoRow(
            label: 'Last Name',
            value: profile?.lastName ?? 'N/A',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _lastNameController,
          ),
          ProfileInfoRow(
            label: 'Second Last Name',
            value: profile?.secondLastName ?? '',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _secondLastNameController,
          ),
        ],
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
        if (!isEditing)
          ProfileInfoRow(
            label: 'Do you have your own vehicle?',
            value: profile?.hasVehicle == true ? 'Yes' : 'No',
            icon: Icons.directions_car_outlined,
          )
        else
          Padding(
            padding: const EdgeInsets.only(bottom: 12),
            child: Row(
              children: [
                Icon(Icons.directions_car_outlined,
                    size: 20, color: AppTheme.primaryBlue),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Do you have your own vehicle?',
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey.shade600,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Row(
                        children: [
                          Switch(
                            value: _editHasVehicle,
                            onChanged: (v) => setState(() => _editHasVehicle = v),
                            activeThumbColor: AppTheme.primaryBlue,
                          ),
                          Text(
                            _editHasVehicle ? 'Yes' : 'No',
                            style: const TextStyle(
                              fontSize: 14,
                              fontWeight: FontWeight.w600,
                              color: AppTheme.textDark,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
      ],
    );
  }

  Widget _buildContactInfoSection(WorkerProfile? profile) {
    final isEditing = _editingSection == ProfileSection.contact;
    return ProfileSectionCard(
      title: 'Contact Information',
      icon: Icons.contact_phone_outlined,
      iconGradient: const [Color(0xFF4CAF50), Color(0xFF81C784)],
      trailing: _sectionEditActions(ProfileSection.contact, profile),
      children: [
        ProfileInfoRow(
          label: 'Mobile Number',
          value: _formatPhone(profile?.mobileNumber),
          icon: Icons.phone_outlined,
          isEditing: isEditing,
          controller: isEditing ? _mobileNumberController : null,
          inputFormatters: isEditing ? [_mobileMaskFormatter] : null,
        ),
        ProfileInfoRow(
          label: 'Phone',
          value: _formatPhone(profile?.phone),
          icon: Icons.phone_outlined,
          isEditing: isEditing,
          controller: isEditing ? _phoneController : null,
          inputFormatters: isEditing ? [_phoneMaskFormatter] : null,
        ),
        ProfileInfoRow(
          label: 'Email',
          value: profile?.email ?? 'N/A',
          icon: Icons.email_outlined,
        ),
        if (!isEditing) ...[
          ProfileInfoRow(
            label: 'Country',
            value: profile?.country ?? 'N/A',
            icon: Icons.flag_outlined,
          ),
          ProfileInfoRow(
            label: 'State / Province',
            value: profile?.province ?? 'N/A',
            icon: Icons.map_outlined,
          ),
          ProfileInfoRow(
            label: 'City',
            value: profile?.city ?? 'N/A',
            icon: Icons.location_city_outlined,
          ),
        ],
        if (isEditing) ...[
          const SizedBox(height: 4),
          Text(
            'Location',
            style: TextStyle(
              fontSize: 12,
              color: Colors.grey.shade600,
              fontWeight: FontWeight.w500,
            ),
          ),
          const SizedBox(height: 8),
          LocationSelector(
            selectedCountry: _editCountry,
            selectedProvince: _editProvince,
            selectedCity: _editCity,
            onCountryChanged: (c) => setState(() {
              _editCountry = c;
              _editProvince = null;
              _editCity = null;
            }),
            onProvinceChanged: (p) => setState(() {
              _editProvince = p;
              _editCity = null;
            }),
            onCityChanged: (c) => setState(() => _editCity = c),
          ),
          const SizedBox(height: 8),
        ],
        ProfileInfoRow(
          label: 'Address',
          value: profile?.address ?? 'N/A',
          icon: Icons.home_outlined,
          isEditing: isEditing,
          controller: isEditing ? _addressController : null,
        ),
        ProfileInfoRow(
          label: 'Postal / ZIP Code',
          value: profile?.postalCode ?? 'N/A',
          icon: Icons.markunread_mailbox_outlined,
          isEditing: isEditing,
          controller: isEditing ? _postalCodeController : null,
        ),
      ],
    );
  }

  Widget _buildSinSection(WorkerProfile? profile) {
    final isEditing = _editingSection == ProfileSection.sin;
    return ProfileSectionCard(
      title: 'SIN / SSN',
      icon: Icons.security_outlined,
      iconGradient: const [Color(0xFFFF9800), Color(0xFFFFB74D)],
      trailing: _sectionEditActions(ProfileSection.sin, profile),
      children: [
        ProfileInfoRow(
          label: 'SIN / SSN #',
          value: isEditing
              ? (profile?.socialInsurance ?? '')
              : profile?.maskedSocialInsurance ?? 'N/A',
          icon: Icons.lock_outlined,
          isEditing: isEditing,
          controller: isEditing ? _socialInsuranceController : null,
        ),
        ProfileInfoRow(
          label: 'Expires',
          value: profile?.socialInsuranceExpire == true ? 'Yes' : 'No',
          icon: Icons.event_busy_outlined,
        ),
        if (profile?.socialInsuranceExpire == true)
          ProfileInfoRow(
            label: 'Due Date',
            value: profile?.formattedDueDate ?? 'N/A',
            icon: Icons.calendar_today_outlined,
          ),
        _buildDocumentFileRow(
          label: 'File',
          fileName: profile?.socialInsuranceFileName,
          fileUrl: profile?.socialInsuranceFileUrl,
          isMarkedForDeletion: _deleteSinFile,
          pendingFile: _replaceSinFile,
          docType: 'sinFile',
          editingSection: ProfileSection.sin,
          onDelete: () => setState(() {
            _deleteSinFile = true;
            _replaceSinFile = null;
          }),
          onUndo: () => setState(() => _deleteSinFile = false),
          onClearPick: () => setState(() => _replaceSinFile = null),
        ),
      ],
    );
  }

  Widget _buildDocumentsSection(WorkerProfile? profile) {
    final isEditing = _editingSection == ProfileSection.documents;
    return ProfileSectionCard(
      title: 'Documents',
      icon: Icons.description_outlined,
      iconGradient: const [Color(0xFF9C27B0), Color(0xFFBA68C8)],
      trailing: _sectionEditActions(ProfileSection.documents, profile),
      children: [
        if (profile?.identificationType1 != null) ...[
          ProfileInfoRow(
            label: '${profile!.identificationType1!} #',
            value: isEditing
                ? (profile.identificationNumber1 ?? '')
                : profile.maskedIdNumber1,
            icon: Icons.credit_card_outlined,
            isEditing: isEditing,
            controller: isEditing ? _idNumber1Controller : null,
          ),
          _buildDocumentFileRow(
            label: '${profile.identificationType1!} (File)',
            fileName: profile.identificationType1FileName,
            fileUrl: profile.identificationType1FileUrl,
            isMarkedForDeletion: _deleteId1File,
            pendingFile: _replaceId1File,
            docType: 'id1File',
            editingSection: ProfileSection.documents,
            onDelete: () => setState(() {
              _deleteId1File = true;
              _replaceId1File = null;
            }),
            onUndo: () => setState(() => _deleteId1File = false),
            onClearPick: () => setState(() => _replaceId1File = null),
          ),
        ],
        if (profile?.identificationType2 != null) ...[
          ProfileInfoRow(
            label: '${profile!.identificationType2!} #',
            value: isEditing
                ? (profile.identificationNumber2 ?? '')
                : profile.maskedIdNumber2,
            icon: Icons.credit_card_outlined,
            isEditing: isEditing,
            controller: isEditing ? _idNumber2Controller : null,
          ),
          _buildDocumentFileRow(
            label: '${profile.identificationType2!} (File)',
            fileName: profile.identificationType2FileName,
            fileUrl: profile.identificationType2FileUrl,
            isMarkedForDeletion: _deleteId2File,
            pendingFile: _replaceId2File,
            docType: 'id2File',
            editingSection: ProfileSection.documents,
            onDelete: () => setState(() {
              _deleteId2File = true;
              _replaceId2File = null;
            }),
            onUndo: () => setState(() => _deleteId2File = false),
            onClearPick: () => setState(() => _replaceId2File = null),
          ),
        ],
        if (profile?.identificationType1 == null &&
            profile?.identificationType2 == null)
          Text(
            'No documents on file',
            style: TextStyle(
              fontSize: 14,
              color: Colors.grey.shade500,
            ),
          ),
      ],
    );
  }

  // ── Preferences sections ──────────────────────────────────────────────────

  Widget _buildPreferencesSection(WorkerProfile? profile) {
    final isEditing = _editingSection == ProfileSection.preferences;
    if (!isEditing) {
      return ProfileSectionCard(
        title: 'Preferences',
        icon: Icons.work_outline,
        iconGradient: const [Color(0xFF2196F3), Color(0xFF64B5F6)],
        trailing: _sectionEditActions(ProfileSection.preferences, profile),
        children: [
          _buildChipDisplayRow(
            label: 'Can you lift up to',
            icon: Icons.fitness_center_outlined,
            chips: profile?.liftCapacity != null ? [profile!.liftCapacity!] : [],
          ),
          _buildChipDisplayRow(
            label: 'Availability',
            icon: Icons.schedule_outlined,
            chips: profile?.availabilities ?? [],
          ),
          _buildChipDisplayRow(
            label: 'Available Time',
            icon: Icons.access_time_outlined,
            chips: profile?.availabilityTimes ?? [],
          ),
          _buildChipDisplayRow(
            label: 'Available Days',
            icon: Icons.calendar_today_outlined,
            chips: profile?.availabilityDays ?? [],
          ),
          _buildChipDisplayRow(
            label: 'Skills',
            icon: Icons.stars_outlined,
            chips: profile?.skills ?? [],
          ),
          _buildChipDisplayRow(
            label: 'Languages',
            icon: Icons.language_outlined,
            chips: profile?.languages ?? [],
          ),
        ],
      );
    }

    return ProfileSectionCard(
      title: 'Preferences',
      icon: Icons.work_outline,
      iconGradient: const [Color(0xFF2196F3), Color(0xFF64B5F6)],
      trailing: _sectionEditActions(ProfileSection.preferences, profile),
      children: [
        _buildChipSelector(
          label: 'Can you lift up to',
          asyncValue: ref.watch(liftingCapacitiesProvider),
          selectedIds: _editLiftId != null ? {_editLiftId!} : {},
          singleSelect: true,
          onToggle: (id, selected) => setState(() {
            _editLiftId = selected ? id : null;
          }),
        ),
        const SizedBox(height: 12),
        _buildChipSelector(
          label: 'Availability',
          asyncValue: ref.watch(availabilityListProvider),
          selectedIds: _editAvailabilityIds,
          singleSelect: false,
          onToggle: (id, selected) => setState(() {
            if (selected) {
              _editAvailabilityIds.add(id);
            } else {
              _editAvailabilityIds.remove(id);
            }
          }),
        ),
        const SizedBox(height: 12),
        _buildChipSelector(
          label: 'Available Time',
          asyncValue: ref.watch(availabilityTimeListProvider),
          selectedIds: _editAvailabilityTimeIds,
          singleSelect: false,
          onToggle: (id, selected) => setState(() {
            if (selected) {
              _editAvailabilityTimeIds.add(id);
            } else {
              _editAvailabilityTimeIds.remove(id);
            }
          }),
        ),
        const SizedBox(height: 12),
        _buildChipSelector(
          label: 'Available Days',
          asyncValue: ref.watch(daysOfWeekProvider),
          selectedIds: _editAvailabilityDayIds,
          singleSelect: false,
          onToggle: (id, selected) => setState(() {
            if (selected) {
              _editAvailabilityDayIds.add(id);
            } else {
              _editAvailabilityDayIds.remove(id);
            }
          }),
        ),
        const SizedBox(height: 12),
        SearchableToggleList(
          label: 'Skills',
          asyncValue: ref.watch(skillsProvider),
          selectedIds: _editSkillIds,
          onToggle: (id, selected) => setState(() {
            if (selected) {
              _editSkillIds.add(id);
            } else {
              _editSkillIds.remove(id);
            }
          }),
        ),
        const SizedBox(height: 12),
        SearchableToggleList(
          label: 'Languages',
          asyncValue: ref.watch(languagesProvider),
          selectedIds: _editLanguageIds,
          onToggle: (id, selected) => setState(() {
            if (selected) {
              _editLanguageIds.add(id);
            } else {
              _editLanguageIds.remove(id);
            }
          }),
        ),
      ],
    );
  }

  Widget _buildEmergencyInfoSection(WorkerProfile? profile) {
    final isEditing = _editingSection == ProfileSection.emergency;
    return ProfileSectionCard(
      title: 'Emergency Information',
      icon: Icons.emergency_outlined,
      iconGradient: const [Color(0xFFF44336), Color(0xFFE57373)],
      trailing: _sectionEditActions(ProfileSection.emergency, profile),
      children: [
        ProfileInfoRow(
          label: 'Do you have any health problems / allergies?',
          value: profile?.haveAnyHealthProblem == true ? 'Yes' : 'No',
          icon: Icons.health_and_safety_outlined,
        ),
        const Padding(
          padding: EdgeInsets.only(bottom: 8),
          child: Text(
            'In case of emergency notify:',
            style: TextStyle(
              fontSize: 12,
              color: AppTheme.primaryBlue,
              fontStyle: FontStyle.italic,
              fontWeight: FontWeight.w500,
            ),
          ),
        ),
        ProfileInfoRow(
          label: 'Name',
          value: profile != null
              ? '${profile.contactEmergencyName ?? ''} ${profile.contactEmergencyLastName ?? ''}'.trim()
              : 'N/A',
          icon: Icons.person_outline,
          isEditing: isEditing,
          controller: isEditing ? _emergencyNameController : null,
        ),
        if (isEditing)
          ProfileInfoRow(
            label: 'Last Name',
            value: profile?.contactEmergencyLastName ?? '',
            icon: Icons.person_outline,
            isEditing: true,
            controller: _emergencyLastNameController,
          ),
        ProfileInfoRow(
          label: 'Phone',
          value: _formatPhone(profile?.contactEmergencyPhone),
          icon: Icons.phone_callback_outlined,
          isEditing: isEditing,
          controller: isEditing ? _emergencyPhoneController : null,
          inputFormatters: isEditing ? [_emergencyPhoneMaskFormatter] : null,
        ),
      ],
    );
  }

  /// Formats a raw phone number string using the ### ### #### mask.
  String _formatPhone(String? phone) {
    if (phone == null || phone.isEmpty) return 'N/A';
    final digits = phone.replaceAll(RegExp(r'[^0-9]'), '');
    if (digits.isEmpty) return 'N/A';
    final buffer = StringBuffer();
    for (var i = 0; i < digits.length && i < 10; i++) {
      if (i == 3 || i == 6) buffer.write(' ');
      buffer.write(digits[i]);
    }
    return buffer.toString();
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  Widget _buildChipDisplayRow({
    required String label,
    required IconData icon,
    required List<String> chips,
  }) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, size: 20, color: AppTheme.primaryBlue),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: TextStyle(
                    fontSize: 12,
                    color: Colors.grey.shade600,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(height: 6),
                chips.isEmpty
                    ? Text(
                        'N/A',
                        style: const TextStyle(
                          fontSize: 15,
                          fontWeight: FontWeight.w600,
                          color: AppTheme.textDark,
                        ),
                      )
                    : Wrap(
                        spacing: 6,
                        runSpacing: 4,
                        children: chips
                            .map(
                              (c) => Chip(
                                label: Text(
                                  c,
                                  style: const TextStyle(
                                    fontSize: 12,
                                    color: AppTheme.primaryBlue,
                                    fontWeight: FontWeight.w500,
                                  ),
                                ),
                                backgroundColor:
                                    AppTheme.primaryBlue.withValues(alpha: 0.1),
                                side: BorderSide(
                                  color: AppTheme.primaryBlue.withValues(alpha: 0.3),
                                ),
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 4, vertical: 0),
                                materialTapTargetSize:
                                    MaterialTapTargetSize.shrinkWrap,
                                visualDensity: VisualDensity.compact,
                              ),
                            )
                            .toList(),
                      ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildChipSelector({
    required String label,
    required AsyncValue<dynamic> asyncValue,
    required Set<String> selectedIds,
    required bool singleSelect,
    required void Function(String id, bool selected) onToggle,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: TextStyle(
            fontSize: 12,
            color: Colors.grey.shade600,
            fontWeight: FontWeight.w500,
          ),
        ),
        const SizedBox(height: 6),
        asyncValue.when(
          data: (items) {
            final list = items as List;
            return Wrap(
              spacing: 6,
              runSpacing: 4,
              children: list.map((item) {
                final id = (item.id as String?) ?? '';
                final value = item.value != null
                    ? item.value as String
                    : item.toString();
                final isSelected = selectedIds.contains(id);
                return FilterChip(
                  label: Text(value, style: const TextStyle(fontSize: 12)),
                  selected: isSelected,
                  onSelected: (s) => onToggle(id, s),
                  selectedColor: AppTheme.primaryBlue.withValues(alpha: 0.15),
                  checkmarkColor: AppTheme.primaryBlue,
                  labelStyle: TextStyle(
                    color: isSelected ? AppTheme.primaryBlue : AppTheme.textDark,
                    fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
                  ),
                  side: BorderSide(
                    color: isSelected ? AppTheme.primaryBlue : Colors.grey.shade300,
                  ),
                  padding: const EdgeInsets.symmetric(horizontal: 4, vertical: 0),
                );
              }).toList(),
            );
          },
          loading: () => const SizedBox(
            height: 24,
            width: 24,
            child: CircularProgressIndicator(strokeWidth: 2),
          ),
          error: (_, _) => Text(
            'Failed to load options',
            style: TextStyle(fontSize: 12, color: Colors.grey.shade500),
          ),
        ),
      ],
    );
  }

  Widget _buildDocumentFileRow({
    required String label,
    required String? fileName,
    required String? fileUrl,
    required bool isMarkedForDeletion,
    required PickedFileData? pendingFile,
    required VoidCallback onDelete,
    required VoidCallback onUndo,
    required String docType,
    required VoidCallback onClearPick,
    required ProfileSection editingSection,
  }) {
    final isEditingDocs = _editingSection == editingSection;
    final hasFile = fileName != null && fileName.isNotEmpty;
    final hasPending = pendingFile != null;
    final hasUrl = fileUrl != null && fileUrl.isNotEmpty;

    if (!isEditingDocs) {
      return Padding(
        padding: const EdgeInsets.only(bottom: 12),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(Icons.attach_file_outlined, size: 20, color: AppTheme.primaryBlue),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    label,
                    style: TextStyle(
                      fontSize: 12,
                      color: Colors.grey.shade600,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    hasFile ? label.replaceAll(' (File)', '') : 'Not uploaded',
                    style: TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.w600,
                      color: hasFile ? AppTheme.textDark : Colors.grey,
                    ),
                  ),
                ],
              ),
            ),
            if (hasUrl)
              IconButton(
                onPressed: () => _previewDocument(fileUrl, label),
                icon: const Icon(Icons.visibility_outlined, size: 20),
                color: AppTheme.primaryBlue,
                tooltip: 'Preview',
                padding: EdgeInsets.zero,
                constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
              ),
          ],
        ),
      );
    }

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
                      color: hasPending ? AppTheme.primaryBlue : AppTheme.errorRed,
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

  Widget _buildActionButtons() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Column(
        children: [
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

    if (shouldLogout != true || !mounted) return;

    final idToken = ref.read(authViewModelProvider).token?.idToken;
    final notifier = ref.read(authViewModelProvider.notifier);

    await Navigator.of(context).push(
      MaterialPageRoute<bool>(
        builder: (_) => LogoutWebviewPage(idToken: idToken),
      ),
    );

    await notifier.logout();
    if (mounted) context.go(AppRoutes.welcome);
  }
}

class _DocumentPreviewPage extends StatefulWidget {
  final String url;
  final String title;
  final String? token;

  const _DocumentPreviewPage({
    required this.url,
    required this.title,
    this.token,
  });

  @override
  State<_DocumentPreviewPage> createState() => _DocumentPreviewPageState();
}

class _DocumentPreviewPageState extends State<_DocumentPreviewPage> {
  late final WebViewController _controller;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _controller = WebViewController()
      ..setJavaScriptMode(JavaScriptMode.unrestricted)
      ..setNavigationDelegate(
        NavigationDelegate(
          onPageFinished: (_) => setState(() => _isLoading = false),
        ),
      )
      ..loadRequest(
        Uri.parse(widget.url),
        headers: widget.token != null
            ? {'Authorization': 'Bearer ${widget.token}'}
            : {},
      );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.title),
        backgroundColor: AppTheme.primaryBlue,
        foregroundColor: Colors.white,
      ),
      body: Stack(
        children: [
          WebViewWidget(controller: _controller),
          if (_isLoading)
            const Center(child: CircularProgressIndicator()),
        ],
      ),
    );
  }
}
