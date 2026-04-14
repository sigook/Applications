import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/theme/app_theme.dart';

class SearchableToggleList extends StatefulWidget {
  final String label;
  final AsyncValue<dynamic> asyncValue;
  final Set<String> selectedIds;
  final void Function(String id, bool selected) onToggle;

  /// When provided, shows an "Add '[query]'" button when the search has no
  /// matches, allowing the user to add a custom entry not in the catalog.
  final void Function(String value)? onAddCustom;

  const SearchableToggleList({
    super.key,
    required this.label,
    required this.asyncValue,
    required this.selectedIds,
    required this.onToggle,
    this.onAddCustom,
  });

  @override
  State<SearchableToggleList> createState() => _SearchableToggleListState();
}

class _SearchableToggleListState extends State<SearchableToggleList> {
  final TextEditingController _searchController = TextEditingController();
  String _searchQuery = '';

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          widget.label,
          style: TextStyle(
            fontSize: 12,
            color: Colors.grey.shade600,
            fontWeight: FontWeight.w500,
          ),
        ),
        const SizedBox(height: 6),
        widget.asyncValue.when(
          data: (items) {
            final list = items as List;
            final allItems = list.map((item) {
              final value =
                  item.value != null ? item.value as String : item.toString();
              final id = (item.id as String?)?.isNotEmpty == true
                  ? item.id as String
                  : value;
              return (id: id, value: value);
            }).toList();

            final filteredItems = _searchQuery.isEmpty
                ? allItems
                : allItems
                    .where((item) => item.value
                        .toLowerCase()
                        .contains(_searchQuery.toLowerCase()))
                    .toList();

            final selectedItems = allItems
                .where((item) => widget.selectedIds.contains(item.id))
                .toList();

            // Custom entries: selected IDs that don't exist in the catalog.
            final catalogIds = allItems.map((i) => i.id).toSet();
            final customSelected = widget.selectedIds
                .where((id) => !catalogIds.contains(id))
                .toList();

            return Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Search field
                TextField(
                  controller: _searchController,
                  onChanged: (value) => setState(() => _searchQuery = value),
                  decoration: InputDecoration(
                    hintText: 'Search ${widget.label.toLowerCase()}...',
                    hintStyle: TextStyle(
                      fontSize: 13,
                      color: Colors.grey.shade400,
                    ),
                    prefixIcon: Icon(
                      Icons.search,
                      size: 20,
                      color: Colors.grey.shade500,
                    ),
                    suffixIcon: _searchQuery.isNotEmpty
                        ? IconButton(
                            icon: Icon(
                              Icons.clear,
                              size: 18,
                              color: Colors.grey.shade500,
                            ),
                            onPressed: () {
                              _searchController.clear();
                              setState(() => _searchQuery = '');
                            },
                          )
                        : null,
                    isDense: true,
                    contentPadding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 10,
                    ),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(10),
                      borderSide: BorderSide(color: Colors.grey.shade300),
                    ),
                    enabledBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(10),
                      borderSide: BorderSide(color: Colors.grey.shade300),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(10),
                      borderSide: const BorderSide(
                        color: AppTheme.primaryBlue,
                        width: 1.5,
                      ),
                    ),
                    filled: true,
                    fillColor: Colors.white,
                  ),
                  style: const TextStyle(fontSize: 13),
                ),
                const SizedBox(height: 8),
                // Toggle list
                Container(
                  constraints: const BoxConstraints(maxHeight: 200),
                  decoration: BoxDecoration(
                    border: Border.all(color: Colors.grey.shade200),
                    borderRadius: BorderRadius.circular(10),
                    color: Colors.white,
                  ),
                  child: filteredItems.isEmpty
                      ? Padding(
                          padding: const EdgeInsets.all(12),
                          child: Column(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              Text(
                                _searchQuery.isNotEmpty
                                    ? 'No ${widget.label.toLowerCase()} match "$_searchQuery"'
                                    : 'No ${widget.label.toLowerCase()} available',
                                style: TextStyle(
                                  fontSize: 12,
                                  color: Colors.grey.shade500,
                                ),
                                textAlign: TextAlign.center,
                              ),
                              if (_searchQuery.isNotEmpty &&
                                  widget.onAddCustom != null) ...[
                                const SizedBox(height: 8),
                                TextButton.icon(
                                  onPressed: () {
                                    widget.onAddCustom!(_searchQuery.trim());
                                    _searchController.clear();
                                    setState(() => _searchQuery = '');
                                  },
                                  icon: const Icon(Icons.add, size: 16),
                                  label: Text(
                                    'Add "$_searchQuery"',
                                    style: const TextStyle(fontSize: 12),
                                  ),
                                  style: TextButton.styleFrom(
                                    foregroundColor: AppTheme.primaryBlue,
                                    padding: const EdgeInsets.symmetric(
                                      horizontal: 8,
                                      vertical: 4,
                                    ),
                                  ),
                                ),
                              ],
                            ],
                          ),
                        )
                      : ListView.separated(
                          shrinkWrap: true,
                          padding: const EdgeInsets.symmetric(vertical: 4),
                          itemCount: filteredItems.length,
                          separatorBuilder: (_, _) => Divider(
                            height: 1,
                            color: Colors.grey.shade100,
                          ),
                          itemBuilder: (context, index) {
                            final item = filteredItems[index];
                            final isSelected =
                                widget.selectedIds.contains(item.id);
                            return InkWell(
                              onTap: () =>
                                  widget.onToggle(item.id, !isSelected),
                              borderRadius: BorderRadius.circular(
                                index == 0 || index == filteredItems.length - 1
                                    ? 10
                                    : 0,
                              ),
                              child: Padding(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 12,
                                  vertical: 10,
                                ),
                                child: Row(
                                  children: [
                                    Expanded(
                                      child: Text(
                                        item.value,
                                        style: TextStyle(
                                          fontSize: 13,
                                          color: isSelected
                                              ? AppTheme.primaryBlue
                                              : AppTheme.textDark,
                                          fontWeight: isSelected
                                              ? FontWeight.w600
                                              : FontWeight.normal,
                                        ),
                                      ),
                                    ),
                                    SizedBox(
                                      height: 24,
                                      width: 40,
                                      child: FittedBox(
                                        fit: BoxFit.contain,
                                        child: Switch.adaptive(
                                          value: isSelected,
                                          onChanged: (value) =>
                                              widget.onToggle(item.id, value),
                                          activeTrackColor: AppTheme.primaryBlue,
                                        ),
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            );
                          },
                        ),
                ),
                // Selected chips (catalog + custom)
                if (selectedItems.isNotEmpty || customSelected.isNotEmpty) ...[
                  const SizedBox(height: 10),
                  Wrap(
                    spacing: 6,
                    runSpacing: 4,
                    children: [
                      ...selectedItems.map((item) => Chip(
                            label: Text(
                              item.value,
                              style: const TextStyle(fontSize: 11),
                            ),
                            deleteIcon: const Icon(Icons.close, size: 16),
                            onDeleted: () => widget.onToggle(item.id, false),
                            materialTapTargetSize:
                                MaterialTapTargetSize.shrinkWrap,
                            visualDensity: VisualDensity.compact,
                            backgroundColor:
                                AppTheme.primaryBlue.withValues(alpha: 0.1),
                            deleteIconColor: AppTheme.primaryBlue,
                            labelStyle: const TextStyle(
                              color: AppTheme.primaryBlue,
                              fontWeight: FontWeight.w600,
                            ),
                            side: BorderSide(
                              color: AppTheme.primaryBlue.withValues(alpha: 0.3),
                            ),
                            padding: const EdgeInsets.symmetric(
                              horizontal: 4,
                              vertical: 0,
                            ),
                          )),
                      ...customSelected.map((id) => Chip(
                            label: Text(
                              id,
                              style: const TextStyle(fontSize: 11),
                            ),
                            deleteIcon: const Icon(Icons.close, size: 16),
                            onDeleted: () => widget.onToggle(id, false),
                            materialTapTargetSize:
                                MaterialTapTargetSize.shrinkWrap,
                            visualDensity: VisualDensity.compact,
                            backgroundColor:
                                AppTheme.primaryBlue.withValues(alpha: 0.1),
                            deleteIconColor: AppTheme.primaryBlue,
                            labelStyle: const TextStyle(
                              color: AppTheme.primaryBlue,
                              fontWeight: FontWeight.w600,
                            ),
                            side: BorderSide(
                              color: AppTheme.primaryBlue.withValues(alpha: 0.3),
                            ),
                            padding: const EdgeInsets.symmetric(
                              horizontal: 4,
                              vertical: 0,
                            ),
                          )),
                    ],
                  ),
                ],
              ],
            );
          },
          loading: () => const SizedBox(
            height: 24,
            width: 24,
            child: CircularProgressIndicator(strokeWidth: 2),
          ),
          error: (_, _) => Text(
            'Failed to load ${widget.label.toLowerCase()}',
            style: TextStyle(fontSize: 12, color: Colors.grey.shade500),
          ),
        ),
      ],
    );
  }
}
