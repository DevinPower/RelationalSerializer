<template>
    <!--<nav class="app-navbar">
        <ul class="app-nav-links">
            <li><router-link to="/">Home</router-link></li>
            <li><router-link to="/import">Import</router-link></li>
            <li><router-link to="/templates">Templates</router-link></li>
            <li><router-link to="/analysis">Analysis</router-link></li>
        </ul>
    </nav>

    <div class="app-container">
        <div class="sidebar">
            <ProjectNav @update:project="swapProject"></ProjectNav>
        </div>
        <div class="sidebar" id="sidebar">
            <ObjectNav :project="project"></ObjectNav>
        </div>

        <div class="main-content">
            <router-view />
        </div>
    </div>-->
    <div class="flex min-h-screen w-full">
        <ProjectNav @update:project="swapProject" />
        <ObjectNav :project="project" />
        <main class="flex-5 flex flex-col min-h-screen">
            <BreadcrumbNav />
            <div class="router-view-container grow flex flex-col">
                <router-view class="router-view-content grow" />
            </div>
        </main>
    </div>
</template>

<script>
    import ObjectNav from './components/ObjectNav.vue'
    import ProjectNav from './components/ProjectNav.vue'
    import BreadcrumbNav from './components/BreadcrumbNav.vue';

export default {
  name: 'App',
  components: {
    ObjectNav,
    ProjectNav,
    BreadcrumbNav
  },
  data() {
    return {
      project: 0
    };
  },
  methods: {
    swapProject(index) {
      this.project = index;
    }
  }
}
</script>
<style>
.router-view-container {
  flex: 1 1 0%;
  display: flex;
  flex-direction: column;
  min-height: 0;
}
.router-view-content {
  flex: 1 1 0%;
  min-height: 0;
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>

