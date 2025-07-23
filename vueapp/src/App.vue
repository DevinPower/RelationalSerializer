<template>
    <InitialSetupModal :open="true"> </InitialSetupModal>

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
    import ObjectNav from './components/ObjectNav.vue';
    import ProjectNav from './components/ProjectNav.vue';
    import BreadcrumbNav from './components/BreadcrumbNav.vue';
    import InitialSetupModal from './components/InitialSetupModal.vue';

export default {
  name: 'App',
  components: {
    ObjectNav,
    ProjectNav,
    BreadcrumbNav,
    InitialSetupModal
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

