import { RouteConfig } from 'vue-router';
import Home from "@/pages/landing/Home.vue";

// Lazy loading para páginas secundarias de landing
const JobSeekers = () => import("@/pages/landing/JobSeekers.vue");
const Business = () => import("@/pages/landing/Business.vue");
const DirectHiring = () => import("@/pages/landing/DirectHiring.vue");
const AboutUs = () => import("@/pages/landing/AboutUs.vue");
const Contact = () => import("@/pages/landing/Contact.vue");
const JobSeekersJobPosition = () => import("@/pages/landing/JobSeekersJobPosition.vue");
const BusinessJobPosition = () => import("@/pages/landing/BusinessJobPosition.vue");
const Atas = () => import("@/pages/landing/Atas.vue");
const TermsAndConditions = () => import("@/pages/landing/TermsAndConditions.vue");
const PrivacyPolicy = () => import("@/pages/landing/PrivacyPolicy.vue");

const routesLanding: RouteConfig[] = [
    {
        path: "/home",
        name: "home",
        component: Home
    },
    {
        path: "/jobSeekers",
        name: "jobSeekers",
        component: JobSeekers,
    },
    {
        path: '/jobSeekers/:position',
        name: 'job-seekers-job-position',
        component: JobSeekersJobPosition
    },
    {
        path: "/business",
        name: "business",
        component: Business
    },
    {
        path: "/business/:position",
        name: "business-job-position",
        component: BusinessJobPosition
    },
    {
        path: "/direct-hiring",
        name: "direct-hiring",
        component: DirectHiring
    },
    {
        path: "/about-us",
        name: "about-us",
        component: AboutUs
    },
    {
        path: "/contact",
        name: "contact",
        component: Contact
    },
    {
        path: "/atas",
        name: "atas",
        component: Atas
    },
    {
        path: "/terms-and-conditions",
        name: "terms-and-conditions",
        component: TermsAndConditions
    },
    {
        path: "/privacy-policy",
        name: "privacy-policy",
        component: PrivacyPolicy
    }
];

export default routesLanding;
