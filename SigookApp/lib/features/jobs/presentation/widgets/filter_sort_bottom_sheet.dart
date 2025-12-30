import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

enum SortOption {
  dateNewest,
  dateOldest,
  rateHighest,
  rateLowest,
  workersHighest,
  workersLowest,
}

enum FilterStatus { all, open, booked, pending, cancelled }

class FilterSortBottomSheet extends StatefulWidget {
  final SortOption currentSort;
  final FilterStatus currentFilter;
  final Function(SortOption, FilterStatus) onApply;

  const FilterSortBottomSheet({
    super.key,
    required this.currentSort,
    required this.currentFilter,
    required this.onApply,
  });

  @override
  State<FilterSortBottomSheet> createState() => _FilterSortBottomSheetState();
}

class _FilterSortBottomSheetState extends State<FilterSortBottomSheet> {
  late SortOption _selectedSort;
  late FilterStatus _selectedFilter;

  @override
  void initState() {
    super.initState();
    _selectedSort = widget.currentSort;
    _selectedFilter = widget.currentFilter;
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.only(
          topLeft: Radius.circular(24),
          topRight: Radius.circular(24),
        ),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          const SizedBox(height: 12),
          Container(
            width: 40,
            height: 4,
            decoration: BoxDecoration(
              color: Colors.grey.shade300,
              borderRadius: BorderRadius.circular(2),
            ),
          ),
          const SizedBox(height: 20),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 24),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  'Filter & Sort',
                  style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                    color: AppTheme.textDark,
                  ),
                ),
                TextButton(
                  onPressed: () {
                    setState(() {
                      _selectedSort = SortOption.dateNewest;
                      _selectedFilter = FilterStatus.all;
                    });
                  },
                  child: const Text('Reset'),
                ),
              ],
            ),
          ),
          const SizedBox(height: 8),
          Flexible(
            child: SingleChildScrollView(
              padding: const EdgeInsets.symmetric(horizontal: 24),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Sort By',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                      color: AppTheme.textDark,
                    ),
                  ),
                  const SizedBox(height: 12),
                  _buildSortOption(
                    'Date: Newest First',
                    Icons.arrow_downward,
                    SortOption.dateNewest,
                  ),
                  _buildSortOption(
                    'Date: Oldest First',
                    Icons.arrow_upward,
                    SortOption.dateOldest,
                  ),
                  _buildSortOption(
                    'Pay Rate: Highest',
                    Icons.attach_money,
                    SortOption.rateHighest,
                  ),
                  _buildSortOption(
                    'Pay Rate: Lowest',
                    Icons.money_off,
                    SortOption.rateLowest,
                  ),
                  _buildSortOption(
                    'Workers: Most Needed',
                    Icons.people,
                    SortOption.workersHighest,
                  ),
                  _buildSortOption(
                    'Workers: Least Needed',
                    Icons.person,
                    SortOption.workersLowest,
                  ),
                  const SizedBox(height: 24),
                  const Text(
                    'Filter by Status',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                      color: AppTheme.textDark,
                    ),
                  ),
                  const SizedBox(height: 12),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      _buildFilterChip('All', FilterStatus.all, Colors.grey),
                      _buildFilterChip(
                        'Open',
                        FilterStatus.open,
                        AppTheme.primaryBlue,
                      ),
                      _buildFilterChip(
                        'Booked',
                        FilterStatus.booked,
                        AppTheme.successGreen,
                      ),
                      _buildFilterChip(
                        'Pending',
                        FilterStatus.pending,
                        Colors.orange,
                      ),
                      _buildFilterChip(
                        'Cancelled',
                        FilterStatus.cancelled,
                        AppTheme.errorRed,
                      ),
                    ],
                  ),
                  const SizedBox(height: 32),
                ],
              ),
            ),
          ),
          Container(
            padding: const EdgeInsets.all(24),
            decoration: BoxDecoration(
              color: Colors.white,
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withValues(alpha: 0.05),
                  blurRadius: 10,
                  offset: const Offset(0, -2),
                ),
              ],
            ),
            child: SafeArea(
              child: SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  onPressed: () {
                    widget.onApply(_selectedSort, _selectedFilter);
                    Navigator.of(context).pop();
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: AppTheme.primaryBlue,
                    padding: const EdgeInsets.symmetric(vertical: 16),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  child: const Text(
                    'Apply Filters',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                      color: Colors.white,
                    ),
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSortOption(String title, IconData icon, SortOption option) {
    final isSelected = _selectedSort == option;
    return InkWell(
      onTap: () {
        setState(() {
          _selectedSort = option;
        });
      },
      borderRadius: BorderRadius.circular(12),
      child: Container(
        margin: const EdgeInsets.only(bottom: 8),
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: BoxDecoration(
          color: isSelected
              ? AppTheme.primaryBlue.withValues(alpha: 0.1)
              : Colors.grey.shade50,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: isSelected ? AppTheme.primaryBlue : Colors.grey.shade200,
            width: isSelected ? 2 : 1,
          ),
        ),
        child: Row(
          children: [
            Icon(
              icon,
              color: isSelected ? AppTheme.primaryBlue : AppTheme.textLight,
              size: 20,
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Text(
                title,
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: isSelected ? FontWeight.w600 : FontWeight.w400,
                  color: isSelected ? AppTheme.primaryBlue : AppTheme.textDark,
                ),
              ),
            ),
            if (isSelected)
              const Icon(
                Icons.check_circle,
                color: AppTheme.primaryBlue,
                size: 22,
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildFilterChip(String label, FilterStatus status, Color color) {
    final isSelected = _selectedFilter == status;
    return FilterChip(
      label: Text(label),
      selected: isSelected,
      onSelected: (selected) {
        setState(() {
          _selectedFilter = status;
        });
      },
      backgroundColor: Colors.grey.shade100,
      selectedColor: color.withValues(alpha: 0.15),
      checkmarkColor: color,
      labelStyle: TextStyle(
        color: isSelected ? color : AppTheme.textDark,
        fontWeight: isSelected ? FontWeight.w600 : FontWeight.w500,
        fontSize: 14,
      ),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
        side: BorderSide(
          color: isSelected ? color : Colors.grey.shade300,
          width: isSelected ? 2 : 1,
        ),
      ),
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
    );
  }
}
