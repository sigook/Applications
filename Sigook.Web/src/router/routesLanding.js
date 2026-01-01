import Home from "@/pages/landing/Home";

// Lazy loading para páginas secundarias de landing - chunks se cargan solo cuando se necesitan
const JobSeekers = () => import("@/pages/landing/JobSeekers");
const Business = () => import("@/pages/landing/Business");
const DirectHiring = () => import("@/pages/landing/DirectHiring");
const AboutUs = () => import("@/pages/landing/AboutUs");
const Contact = () => import("@/pages/landing/Contact");
const JobSeekersJobPosition = () => import("@/pages/landing/JobSeekersJobPosition");
const BusinessJobPosition = () => import("@/pages/landing/BusinessJobPosition");
const Atas = () => import("@/pages/landing/Atas");
const TermsAndConditions = () => import("@/pages/landing/TermsAndConditions");
const PrivacyPolicy = () => import("@/pages/landing/PrivacyPolicy");

let routesLanding = [
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