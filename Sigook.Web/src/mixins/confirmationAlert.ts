import { NavigationGuard } from 'vue-router';

export default {
  data() {
    return {
      unsavedChanges: false
    };
  },
  beforeRouteLeave(to: Parameters<NavigationGuard>[0], from: Parameters<NavigationGuard>[1], next: Parameters<NavigationGuard>[2]) {
    if ((this as any).unsavedChanges) {
      const answer = window.confirm('Do you really want to leave? you have unsaved changes!');
      if (answer) {
        next();
      } else {
        next(false);
      }
    } else {
      next();
    }
  }
};
