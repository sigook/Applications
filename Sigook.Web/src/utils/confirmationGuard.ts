import { NavigationGuard } from 'vue-router';

interface ComponentWithUnsavedChanges {
  unsavedChanges: boolean;
}

/**
 * Route guard that warns the user about unsaved changes before leaving.
 * Components using this guard must define `unsavedChanges: boolean` in their data.
 *
 * Usage:
 *   import { confirmationGuard } from '@/utils/confirmationGuard';
 *   export default {
 *     data() { return { unsavedChanges: false }; },
 *     beforeRouteLeave: confirmationGuard,
 *   };
 */
export const confirmationGuard: NavigationGuard = function (to, from, next) {
  const self = this as unknown as ComponentWithUnsavedChanges;
  if (self.unsavedChanges) {
    const answer = window.confirm('Do you really want to leave? you have unsaved changes!');
    if (answer) {
      next();
    } else {
      next(false);
    }
  } else {
    next();
  }
};
