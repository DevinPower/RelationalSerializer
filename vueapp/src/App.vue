<template>
    <nav class="app-navbar">
        <ul class="app-nav-links">
            <li><router-link to="/">Home</router-link></li>
            <li><router-link to="/import">Import</router-link></li>
            <li><router-link to="/templates">Templates</router-link></li>
            <li><router-link to="/analysis">Analysis</router-link></li>
        </ul>
    </nav>

    <div class="container">
        <div class="sidebar">
            <ProjectNav @update:project="swapProject"></ProjectNav>
        </div>
        <div class="sidebar" id="sidebar">
            <ObjectNav :project="project"></ObjectNav>
        </div>

        <div class="main-content">
            <router-view />
        </div>
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

<style>
    .modal-mask {
        position: fixed;
        z-index: 9998;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(0, 0, 0, .5);
        display: table;
        transition: opacity 0.3s ease;
    }

    .modal-wrapper {
        display: table-cell;
        vertical-align: middle;
    }

    .modal-container {
        width: 300px;
        height:100%;
        margin: 0px auto;
        padding: 20px 30px;
        background-color: #fff;
        border-radius: 2px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
        transition: all 0.3s ease;
        font-family: Helvetica, Arial, sans-serif;
    }

    .modal-container h2{
        color:black;
    }

    body {
        margin: 0;
        padding: 0;
        font-family: Arial, sans-serif;
        color: #CCCCCC;
    }

    .container {
        display: flex;
        height: calc(100vh - 48px);
        overflow: hidden;
        width:100vw;
    }

    .sidebar {
        background-color: #333333;
        width: 20vw;
        resize: horizontal;
        overflow: hidden;
        padding: 10px;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.3); /* Add drop shadow */
    }

    .main-content {
        flex: 1;
        padding: 20px;
        background-color: #1A1A1A;
        overflow-y: scroll;
        box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1); /* Add drop shadow */
        z-index: 1; /* Add z-index to make the main content appear above the sidebar */
    }

    .app-navbar {
        margin: 0;
        padding: 0;
        list-style: none;
        text-decoration: none;
        margin-top: 0px;
        background-color: #555555;
        color: #fff;
        padding: 0px 0px;
        display: flex;
        align-items: center;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        height:48px;
    }

    .app-nav-links {
        display: flex;
        list-style: none;
    }

        .app-nav-links li {
            margin-right: 20px;
        }

        .app-nav-links a {
            color: #fff;
            font-weight: 600;
            font-size: 16px;
            text-decoration: none;
        }

            /* Style hover effect for app navigation links */
            .app-nav-links a:hover {
                text-decoration: underline;
            }

    .sidebar ul {
        list-style: none;
        padding: 0;
        margin: 20px;
    }

    .sidebar li {
        margin-bottom: 10px;
    }

    .sidebar a {
        text-decoration: none;
        color: #CCCCCC;
    }

        .sidebar a:hover {
            color: #555;
        }

    /* Style for the resizable handle */
    .sidebar::-webkit-resizer {
        display: block;
        width: 6px;
        background-color: #ddd;
        cursor: ew-resize;
    }

    .sidebar::-moz-resizer {
        display: block;
        width: 6px;
        background-color: #ddd;
        cursor: ew-resize;
    }

    .sidebar::resizer {
        display: block;
        width: 6px;
        background-color: #ddd;
        cursor: ew-resize;
    }

    /* Reset some default styles for input elements */
    input {
        border: none;
        background-color: #272727;
        outline: none;
        font-size: 16px;
        padding: 10px;
        border-radius: 6px;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        transition: box-shadow 0.3s ease;
        position: relative; /* Add position relative for the ::before pseudo-element */
        width: 100%;
        color: #CCCCCC;
    }

        /* Add a custom border when input is focused */
        input:focus {
            border: 2px solid #007BFF;
        }

            /* Add the circle icon inside the input when focused */
            input:focus::before {
                content: '';
                position: absolute;
                top: 50%;
                left: 12px;
                transform: translateY(-50%);
                color: #007BFF;
                font-size: 12px;
                font-weight: bold;
                border: 2px solid #007BFF;
                border-radius: 50%;
                padding: 4px;
            }

        /* Adjust the input width to fit the icon */
        input:focus {
            padding-left: 30px;
        }

    .vl {
        border-left: 2px groove black;
        height:40px;
        float: right;
    }
</style>