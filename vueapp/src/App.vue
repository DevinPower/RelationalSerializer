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
        <ProjectNav @update:project="swapProject"></ProjectNav>
        <ObjectNav :project="project"></ObjectNav>
        <main class="flex-5">
            <div style="padding:32px;">
                <router-view />
            </div>
        </main>
    </div>
</template>

<script>
    import ObjectNav from './components/ObjectNav.vue'
    import ProjectNav from './components/ProjectNav.vue'

export default {
  name: 'App',
  components: {
      ObjectNav,
      ProjectNav
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

