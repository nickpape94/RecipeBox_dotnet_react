import axios from 'axios';
import { GET_USER_FAVOURITES, USER_ERROR } from './types';

export const getFavourites = (id) => async (dispatch) => {
	try {
		const res = await axios.get(`/api/favourites/userId/${id}`);

		dispatch({
			type: GET_USER_FAVOURITES,
			payload: res.data
		});
	} catch (err) {
		dispatch({
			type: USER_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};
