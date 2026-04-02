import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

class RegistrationStepIndicator extends StatelessWidget {
  const RegistrationStepIndicator({
    super.key,
    required this.currentStep,
    required this.completedSteps,
    required this.onStepTapped,
  });

  final int currentStep;
  final List<bool> completedSteps;
  final ValueChanged<int> onStepTapped;

  static const _totalSteps = 4;
  static const _stepTitles = [
    'Basic Information',
    'Preferences',
    'Documents',
    'Account Setup',
  ];

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.fromLTRB(16, 0, 16, 8),
      padding: const EdgeInsets.fromLTRB(28, 20, 28, 18),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppTheme.radiusLarge),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          // ── Dots and connecting lines
          Row(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: _buildDotsAndLines(),
          ),
          const SizedBox(height: 16),
          // ── Current step title
          Text(
            _stepTitles[currentStep],
            style: const TextStyle(
              fontSize: 17,
              fontWeight: FontWeight.w700,
              color: AppTheme.textDark,
              letterSpacing: -0.2,
            ),
          ),
          const SizedBox(height: 2),
          // ── Step counter
          Text(
            'Step ${currentStep + 1} of $_totalSteps',
            style: TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w500,
              color: AppTheme.textMedium.withValues(alpha: 0.7),
            ),
          ),
        ],
      ),
    );
  }

  List<Widget> _buildDotsAndLines() {
    final widgets = <Widget>[];
    for (var i = 0; i < _totalSteps; i++) {
      widgets.add(_buildDot(i));
      if (i < _totalSteps - 1) {
        widgets.add(_buildLine(i));
      }
    }
    return widgets;
  }

  Widget _buildDot(int index) {
    final isActive = index == currentStep;
    final isCompleted = index < currentStep && completedSteps[index];

    final double dotSize = isActive ? 36.0 : 28.0;

    BoxDecoration decoration;
    Widget content;

    if (isCompleted) {
      decoration = const BoxDecoration(
        shape: BoxShape.circle,
        color: AppTheme.primaryBlue,
      );
      content = const Icon(
        Icons.check_rounded,
        color: Colors.white,
        size: 18,
      );
    } else if (isActive) {
      decoration = BoxDecoration(
        shape: BoxShape.circle,
        color: AppTheme.primaryBlue,
        boxShadow: [
          BoxShadow(
            color: AppTheme.primaryBlue.withValues(alpha: 0.35),
            blurRadius: 12,
            spreadRadius: 1,
          ),
        ],
      );
      content = Text(
        '${index + 1}',
        style: const TextStyle(
          color: Colors.white,
          fontSize: 15,
          fontWeight: FontWeight.w700,
        ),
      );
    } else {
      decoration = BoxDecoration(
        shape: BoxShape.circle,
        color: Colors.white,
        border: Border.all(color: AppTheme.borderLight, width: 1.5),
      );
      content = Text(
        '${index + 1}',
        style: const TextStyle(
          color: AppTheme.textMedium,
          fontSize: 13,
          fontWeight: FontWeight.w600,
        ),
      );
    }

    return GestureDetector(
      onTap: () => onStepTapped(index),
      child: SizedBox(
        width: 36,
        height: 36,
        child: Center(
          child: AnimatedContainer(
            duration: const Duration(milliseconds: 200),
            curve: Curves.easeOut,
            width: dotSize,
            height: dotSize,
            decoration: decoration,
            alignment: Alignment.center,
            child: content,
          ),
        ),
      ),
    );
  }

  Widget _buildLine(int afterStepIndex) {
    final isCompleted = currentStep > afterStepIndex;
    return Expanded(
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        height: 2.5,
        margin: const EdgeInsets.symmetric(horizontal: 2),
        decoration: BoxDecoration(
          color: isCompleted ? AppTheme.primaryBlue : AppTheme.borderLight,
          borderRadius: BorderRadius.circular(2),
        ),
      ),
    );
  }
}
