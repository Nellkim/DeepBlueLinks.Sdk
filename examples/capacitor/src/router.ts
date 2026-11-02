import { showRoute } from './view'

// Stands in for your app's navigation, the guide's `router`: the example shows the path it would open.
export const router = {
  open(path: string) {
    showRoute(path)
  },
}
