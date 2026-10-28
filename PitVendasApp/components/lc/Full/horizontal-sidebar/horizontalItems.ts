;export interface menu {
    header?: string;
    title?: string;
    icon?: any;
    to?: string;
    divider?: boolean;
    chip?: string;
    chipColor?: string;
    chipVariant?: string;
    chipIcon?: string;
    children?: menu[];
    disabled?: boolean;
    subCaption?: string;
    class?: string;
    extraclass?: string;
    type?: string;
    BgColor?: string;
}

const horizontalItems: menu[] = [

    {
        title: 'Sample Page',
        icon: 'screencast-2-linear',
        BgColor: 'primary',
        to: '/'
    },
    {
        title: 'Multilevel',
        icon: 'double-alt-arrow-down-bold-duotone',
        BgColor: 'secondary',
        to: '#',
        children: [
            {
                title: 'Level 1',
                to: '/auth/404'
            },
            {
                title: 'Level 1',
                to: '/auth/404',
                children: [
                    {
                        title: 'Level 2',
                        to: '/auth/404'
                    },
                    {
                        title: 'Level 2',
                        to: '/auth/404',
                        children: [
                            {
                                title: 'Level 3',
                                to: '/auth/404'
                            }
                        ]
                    }
                ]
            }
        ]
    },

   


];

export default horizontalItems;
