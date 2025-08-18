import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    mode: 'light',
    primary: { main: '#1976d2' },
    secondary: { main: '#7b1fa2' }
  },
  shape: { borderRadius: 12 },
  components: { MuiPaper: { styleOverrides: { root: { borderRadius: 16 } } } }
});

export default theme;